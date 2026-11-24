// rev 2026-11-23

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Razor;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Runtime.Loader;
using System.Text;

namespace Ans.Net10.Web.Compilation
{

    /// <summary>
    /// Высокопроизводительный рантайм-компилятор Razor-представлений (<c>.cshtml</c>) 
    /// на базе <see cref="RazorProjectEngine"/> и Roslyn (<see cref="CSharpCompilation"/>),
    /// полностью заменяющий устаревший механизм <c>Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation</c>.
    /// </summary>
    public sealed partial class AnsRazorViewCompiler
        : IViewCompiler,
        IDisposable
    {

        private readonly IWebHostEnvironment _env;
        private readonly AnsMetadataReferenceProvider _referenceProvider;
        private readonly ApplicationPartManager _partManager;
        private readonly IServiceProvider _services;
        private readonly ILogger<AnsRazorViewCompiler> _logger;
        private readonly ConcurrentDictionary<string, _CachedViewEntry> _cache = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _compileLocks = new(StringComparer.OrdinalIgnoreCase);
        private readonly Lock _engineLock = new();

        private FrozenDictionary<string, CompiledViewDescriptor> _precompiledViews = FrozenDictionary<string, CompiledViewDescriptor>.Empty;
        private RazorProjectEngine? _projectEngine;
        private AssemblyLoadContext? _viewsLoadContext;
        private CancellationTokenSource _resetTokenSource = new();
        private bool _disposed;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AnsRazorViewCompiler"/>.
        /// </summary>
        /// <param name="env">Среда выполнения веб-приложения для определения физического корня контента.</param>
        /// <param name="referenceProvider">Провайдер ссылок на метаданные сборок.</param>
        /// <param name="partManager">Менеджер прикладных частей приложения для обнаружения встроенных скомпилированных представлений.</param>
        /// <param name="services">Провайдер сервисов приложения для ленивого разрешения <see cref="AnsAppCodeCompiler"/>.</param>
        /// <param name="logger">Экземпляр логгера компилятора.</param>
        public AnsRazorViewCompiler(
            IWebHostEnvironment env,
            AnsMetadataReferenceProvider referenceProvider,
            ApplicationPartManager partManager,
            IServiceProvider services,
            ILogger<AnsRazorViewCompiler> logger)
        {
            ArgumentNullException.ThrowIfNull(env);
            ArgumentNullException.ThrowIfNull(referenceProvider);
            ArgumentNullException.ThrowIfNull(partManager);
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(logger);

            _env = env;
            _referenceProvider = referenceProvider;
            _partManager = partManager;
            _services = services;
            _logger = logger;

            _viewsLoadContext = new AssemblyLoadContext($"AnsViews_{Guid.NewGuid():N}", isCollectible: true);
            _precompiledViews = _buildPrecompiledIndex(_partManager);
            _loadPrecompiledViews();
        }


        /* methods */


        /// <summary>
        /// Асинхронно компилирует или извлекает из кэша дескриптор Razor-представления по его виртуальному пути.
        /// </summary>
        /// <param name="relativePath">Относительный виртуальный путь к <c>.cshtml</c>-файлу (например, <c>"/Views/Home/Index.cshtml"</c>).</param>
        /// <returns>Задача, результатом которой является готовый дескриптор <see cref="CompiledViewDescriptor"/>.</returns>
        /// <exception cref="AnsRazorCompilationException">Вызывается при обнаружении ошибок синтаксиса Razor или C# в представлении.</exception>
        public async Task<CompiledViewDescriptor> CompileAsync(
            string relativePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_services.GetService(typeof(AnsAppCodeCompiler)) is AnsAppCodeCompiler appCodeCompiler1)
                await appCodeCompiler1.EnsureCompiledAsync(throwOnError: true).ConfigureAwait(false);

            var normalizedPath1 = _normalizePath(relativePath);

            if (_cache.TryGetValue(normalizedPath1, out var cached1) && _isCacheValid(cached1))
                return cached1.Descriptor;

            var pathLock1 = _compileLocks.GetOrAdd(normalizedPath1, static _ => new SemaphoreSlim(1, 1));
            await pathLock1.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_cache.TryGetValue(normalizedPath1, out cached1) && _isCacheValid(cached1))
                    return cached1.Descriptor;

                var entry1 = await _compileInternalAsync(normalizedPath1).ConfigureAwait(false);
                _cache[normalizedPath1] = entry1;
                return entry1.Descriptor;
            }
            finally
            {
                pathLock1.Release();
            }
        }


        /// <summary>
        /// Принудительно очищает кэш динамически скомпилированных представлений, 
        /// восстанавливает предкомпилированные RCL-представления, пересоздает движок Razor 
        /// с актуальными метаданными сборок и выгружает предыдущий контекст сборок представлений.
        /// </summary>
        public void ResetCache()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            AssemblyLoadContext? oldContext1;
            CancellationTokenSource oldCts1;
            lock (_engineLock)
            {
                oldCts1 = _resetTokenSource;
                _resetTokenSource = new CancellationTokenSource();

                _cache.Clear();
                _precompiledViews = _buildPrecompiledIndex(_partManager);
                _loadPrecompiledViews();
                _projectEngine = null;

                oldContext1 = _viewsLoadContext;
                _viewsLoadContext = new AssemblyLoadContext($"AnsViews_{Guid.NewGuid():N}", isCollectible: true);
            }

            oldCts1.Cancel();
            oldCts1.Dispose();
            oldContext1?.Unload();
            _log.CacheReset(_logger);
        }


        /// <summary>
        /// Освобождает ресурсы компилятора и выгружает динамический контекст сборок представлений.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _cache.Clear();

            foreach (var semaphore1 in _compileLocks.Values)
                semaphore1.Dispose();
            _compileLocks.Clear();

            _resetTokenSource.Dispose();
            _viewsLoadContext?.Unload();
            _viewsLoadContext = null;
        }


        /* privates */


        private static FrozenDictionary<string, CompiledViewDescriptor> _buildPrecompiledIndex(
            ApplicationPartManager partManager)
        {
            var feature1 = new ViewsFeature();
            partManager.PopulateFeature(feature1);

            var dict1 = new Dictionary<string, CompiledViewDescriptor>(StringComparer.OrdinalIgnoreCase);
            foreach (var descriptor1 in feature1.ViewDescriptors)
            {
                var normalizedPath1 = _normalizePath(descriptor1.RelativePath);
                dict1[normalizedPath1] = descriptor1;
            }

            return dict1.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
        }


        private void _loadPrecompiledViews()
        {
            foreach (var (normalizedPath1, descriptor1) in _precompiledViews)
            {
                var physicalPath1 = Path.Combine(
                    _env.ContentRootPath,
                    normalizedPath1.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(physicalPath1))
                    continue;

                _attachExpirationTokens(descriptor1, normalizedPath1);
                _cache[normalizedPath1] = new _CachedViewEntry(
                    descriptor1,
                    [new _TrackedFileStamp(physicalPath1, DateTime.MinValue)],
                    isPrecompiled: true);
            }
        }


        private async Task<_CachedViewEntry> _compileInternalAsync(
            string normalizedPath)
        {
            var engine1 = _getOrCreateProjectEngine();
            var projectItem1 = engine1.FileSystem.GetItem(normalizedPath, fileKind: null);

            if (!projectItem1.Exists)
            {
                if (_precompiledViews.TryGetValue(normalizedPath, out var rclDescriptor1))
                {
                    _attachExpirationTokens(rclDescriptor1, normalizedPath);
                    var rclStamp1 = projectItem1.PhysicalPath is not null
                        ? [new _TrackedFileStamp(projectItem1.PhysicalPath, DateTime.MinValue)]
                        : ImmutableArray<_TrackedFileStamp>.Empty;

                    return new _CachedViewEntry(rclDescriptor1, rclStamp1, isPrecompiled: true);
                }

                var missingDescriptor1 = new CompiledViewDescriptor
                {
                    RelativePath = normalizedPath
                };
                _attachExpirationTokens(missingDescriptor1, normalizedPath);
                var missingStamp1 = projectItem1.PhysicalPath is not null
                    ? [new _TrackedFileStamp(projectItem1.PhysicalPath, DateTime.MinValue)]
                    : ImmutableArray<_TrackedFileStamp>.Empty;

                return new _CachedViewEntry(missingDescriptor1, missingStamp1, isPrecompiled: false);
            }

            var trackedFiles1 = _collectTrackedFiles(engine1, projectItem1);

            byte[] sourceBytes1;
            string sourceCode1;
            using (var stream1 = projectItem1.Read())
            using (var memoryStream1 = new MemoryStream())
            {
                await stream1.CopyToAsync(memoryStream1).ConfigureAwait(false);
                sourceBytes1 = memoryStream1.ToArray();
                sourceCode1 = Encoding.UTF8.GetString(sourceBytes1);
            }

            var (emitResult1, generatedCode1, peStream1) = await Task.Run<(EmitResult, string, MemoryStream)>(() =>
            {
                var codeDocument1 = engine1.Process(projectItem1);
                var csharpDocument1 = codeDocument1.GetCSharpDocument();

                if (csharpDocument1.Diagnostics.Count > 0)
                {
                    var razorErrors1 = csharpDocument1.Diagnostics
                        .Where(static x => x.Severity == RazorDiagnosticSeverity.Error)
                        .Select(x => new AnsCompilationError(
                            x.Span.FilePath ?? normalizedPath,
                            x.Span.LineIndex + 1,
                            x.Span.CharacterIndex + 1,
                            x.Id,
                            x.GetMessage()))
                        .ToArray();

                    if (razorErrors1.Length > 0)
                    {
                        var razorEx1 = new AnsRazorCompilationException(
                            normalizedPath,
                            razorErrors1,
                            csharpDocument1.GeneratedCode,
                            sourceCode1);
                        _log.CompilationFailed(_logger, razorEx1.Message);
                        throw razorEx1;
                    }
                }

                var assemblyName1 = $"AnsView_{Guid.NewGuid():N}";
                var targetPath1 = projectItem1.PhysicalPath ?? normalizedPath;
                var syntaxTree1 = CSharpSyntaxTree.ParseText(
                    csharpDocument1.GeneratedCode,
                    CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest),
                    path: $"{targetPath1}.g.cs",
                    encoding: Encoding.UTF8);

                var compilationOptions1 = new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,
                    nullableContextOptions: NullableContextOptions.Enable);

                var compilation1 = CSharpCompilation.Create(
                    assemblyName1,
                    [syntaxTree1],
                    _referenceProvider.GetAllReferences(),
                    compilationOptions1);

                var emitOptions1 = new EmitOptions(
                    debugInformationFormat: DebugInformationFormat.Embedded);

                var embeddedTexts1 = projectItem1.PhysicalPath is not null
                    ? new[]
                    {
                        EmbeddedText.FromBytes(
                            projectItem1.PhysicalPath,
                            new ArraySegment<byte>(sourceBytes1),
                            SourceHashAlgorithm.Sha256)
                    }
                    : null;

                var stream1 = new MemoryStream();
                var result1 = compilation1.Emit(
                    stream1,
                    options: emitOptions1,
                    embeddedTexts: embeddedTexts1);
                return (result1, csharpDocument1.GeneratedCode, stream1);

            }).ConfigureAwait(false);

            using (peStream1)
            {
                if (!emitResult1.Success)
                {
                    var csharpErrors1 = emitResult1.Diagnostics
                        .Where(static x => x.IsWarningAsError || x.Severity == DiagnosticSeverity.Error)
                        .Select(x =>
                        {
                            var lineSpan1 = x.Location.GetMappedLineSpan();
                            return new AnsCompilationError(
                                string.IsNullOrEmpty(lineSpan1.Path)
                                    ? normalizedPath
                                    : lineSpan1.Path,
                                lineSpan1.StartLinePosition.Line + 1,
                                lineSpan1.StartLinePosition.Character + 1,
                                x.Id,
                                x.GetMessage());
                        })
                        .ToArray();

                    var csharpEx1 = new AnsRazorCompilationException(
                        normalizedPath,
                        csharpErrors1,
                        generatedCode1,
                        sourceCode1);
                    _log.CompilationFailed(_logger, csharpEx1.Message);
                    throw csharpEx1;
                }

                peStream1.Position = 0;
                AssemblyLoadContext loadContext1;
                lock (_engineLock)
                {
                    loadContext1 = _viewsLoadContext
                        ?? throw new ObjectDisposedException(nameof(AnsRazorViewCompiler));
                }

                var compiledAssembly1 = loadContext1.LoadFromStream(peStream1);
                var loader1 = new RazorCompiledItemLoader();
                var compiledItem1 = loader1.LoadItems(compiledAssembly1) is [var firstItem1, ..]
                    ? firstItem1
                    : throw new InvalidOperationException(
                        $"Не удалось извлечь RazorCompiledItem для представления '{normalizedPath}'.");

                var descriptor1 = new CompiledViewDescriptor(compiledItem1);
                _log.ViewCompiled(_logger, normalizedPath);

                _attachExpirationTokens(descriptor1, normalizedPath);

                return new _CachedViewEntry(descriptor1, trackedFiles1, isPrecompiled: false);
            }
        }


        private void _attachExpirationTokens(
            CompiledViewDescriptor descriptor,
            string normalizedPath)
        {
            var tokens1 = descriptor.ExpirationTokens ??= new List<IChangeToken>();
            tokens1.Clear();
            tokens1.Add(new CancellationChangeToken(_resetTokenSource.Token));
            tokens1.Add(_env.ContentRootFileProvider.Watch(normalizedPath));

            var dir1 = Path.GetDirectoryName(normalizedPath)?.Replace('\\', '/');
            while (!string.IsNullOrEmpty(dir1))
            {
                var importPath1 = dir1 == "/"
                    ? "/_ViewImports.cshtml"
                    : $"{dir1}/_ViewImports.cshtml";
                var startPath1 = dir1 == "/"
                    ? "/_ViewStart.cshtml"
                    : $"{dir1}/_ViewStart.cshtml";

                tokens1.Add(_env.ContentRootFileProvider.Watch(importPath1));
                tokens1.Add(_env.ContentRootFileProvider.Watch(startPath1));

                if (dir1 == "/")
                    break;
                var lastSlash1 = dir1.LastIndexOf('/');
                dir1 = lastSlash1 <= 0 ? "/" : dir1[..lastSlash1];
            }
        }
        

        private RazorProjectEngine _getOrCreateProjectEngine()
        {
            var engine1 = _projectEngine;
            if (engine1 is not null)
                return engine1;

            lock (_engineLock)
            {
                engine1 = _projectEngine;
                if (engine1 is not null)
                    return engine1;

                var fileSystem1 = RazorProjectFileSystem.Create(_env.ContentRootPath);
                var configuration1 = RazorConfiguration.Create(
                    RazorLanguageVersion.Latest,
                    "MVC-3.0",
                    extensions: []);

                var references1 = _referenceProvider.GetAllReferences();
                var tagHelperImports1 = _buildTagHelperImports();

                engine1 = RazorProjectEngine.Create(configuration1, fileSystem1, builder =>
                {
                    RazorExtensions.Register(builder);

                    builder.Features.Add(new _MetadataReferenceFeature(references1));
                    builder.Features.Add(new CompilationTagHelperFeature());
                    builder.Features.Add(new DefaultTagHelperDescriptorProvider());

                    builder.AddDefaultImports($"""
					@using System
					@using System.Collections.Generic
					@using System.Linq
					@using System.Threading.Tasks
					@using Microsoft.AspNetCore.Mvc
					@using Microsoft.AspNetCore.Mvc.Rendering
					@using Microsoft.AspNetCore.Mvc.ViewFeatures
					@using Ans.Net10.Common
					@using Ans.Net10.Web
					@inject Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<TModel> Html
					@inject Microsoft.AspNetCore.Mvc.IUrlHelper Url
					@inject Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider
					@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
					{tagHelperImports1}
					""");
                });

                _projectEngine = engine1;
                return engine1;
            }
        }


        private string _buildTagHelperImports()
        {
            var sb1 = new StringBuilder();
            var added1 = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var part1 in _partManager.ApplicationParts)
            {
                var name1 = part1.Name;
                if (string.IsNullOrWhiteSpace(name1)
                    || name1.StartsWith("System.", StringComparison.OrdinalIgnoreCase)
                    || name1.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (added1.Add(name1))
                    sb1.AppendLine($"@addTagHelper *, {name1}");
            }

            return sb1.ToString();
        }

        
        private static bool _isCacheValid(
            _CachedViewEntry entry)
        {
            if (entry.IsPrecompiled || entry.Descriptor.Item is null)
            {
                foreach (var file1 in entry.TrackedFiles)
                    if (File.Exists(file1.PhysicalPath))
                        return false;
                return true;
            }

            foreach (var file1 in entry.TrackedFiles)
            {
                if (!File.Exists(file1.PhysicalPath))
                    return false;
                if (File.GetLastWriteTimeUtc(file1.PhysicalPath) != file1.LastWriteUtc)
                    return false;
            }

            return true;
        }


        private static ImmutableArray<_TrackedFileStamp> _collectTrackedFiles(
            RazorProjectEngine engine,
            RazorProjectItem mainItem)
        {
            var stamps1 = new List<_TrackedFileStamp>();

            if (mainItem.PhysicalPath is not null
                && File.Exists(mainItem.PhysicalPath))
                stamps1.Add(new _TrackedFileStamp(
                    mainItem.PhysicalPath,
                    File.GetLastWriteTimeUtc(mainItem.PhysicalPath)));

            foreach (var importItem1 in engine.FileSystem.FindHierarchicalItems(mainItem.FilePath, "_ViewImports.cshtml"))
                if (importItem1.Exists
                    && importItem1.PhysicalPath is not null
                    && File.Exists(importItem1.PhysicalPath))
                    stamps1.Add(new _TrackedFileStamp(
                        importItem1.PhysicalPath,
                        File.GetLastWriteTimeUtc(importItem1.PhysicalPath)));

            return [.. stamps1];
        }


        private static string _normalizePath(
            string path)
        {
            var clean1 = path.Replace('\\', '/');
            return clean1.StartsWith('/')
                ? clean1
                : $"/{clean1}";
        }


        /* privates classes */


        private sealed record _TrackedFileStamp(
            string PhysicalPath,
            DateTime LastWriteUtc);


        private sealed class _CachedViewEntry(
            CompiledViewDescriptor descriptor,
            ImmutableArray<_TrackedFileStamp> trackedFiles,
            bool isPrecompiled)
        {
            public CompiledViewDescriptor Descriptor { get; } = descriptor;
            public ImmutableArray<_TrackedFileStamp> TrackedFiles { get; } = trackedFiles;
            public bool IsPrecompiled { get; } = isPrecompiled;
        }


        private sealed class _MetadataReferenceFeature(
            IReadOnlyList<MetadataReference> references)
            : IMetadataReferenceFeature
        {
            public RazorEngine Engine { get; set; } = default!;
            public IReadOnlyList<MetadataReference> References { get; } = references;
        }


        /* logs */


        private static partial class _log
        {
            [LoggerMessage(
                Level = LogLevel.Debug,
                Message = "Razor-представление '{ViewPath}' успешно скомпилировано в рантайме.")]
            public static partial void ViewCompiled(
                ILogger logger, string viewPath);

            [LoggerMessage(
                Level = LogLevel.Error,
                Message = "{Details}")]
            public static partial void CompilationFailed(
                ILogger logger, string details);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "Кэш скомпилированных Razor-представлений сброшен.")]
            public static partial void CacheReset(
                ILogger logger);
        }

    }

}
