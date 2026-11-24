// rev 2026-11-23

using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Runtime.Loader;
using System.Text;

namespace Ans.Net10.Web.Compilation
{

    /// <summary>
    /// Представляет результат компиляции исходных <c>.cs</c>-файлов каталога <c>App_Code</c>.
    /// </summary>
    /// <param name="Success">Признак успешного завершения компиляции.</param>
    /// <param name="SourceFilesCount">Количество обработанных <c>.cs</c>-файлов.</param>
    /// <param name="ExportedTypesCount">Количество публичных типов, экспортированных из скомпилированной сборки.</param>
    /// <param name="ElapsedMilliseconds">Время выполнения компиляции в миллисекундах.</param>
    /// <param name="Errors">Список ошибок компиляции, если сборка завершилась неудачно.</param>
    /// <param name="CompiledAtUtc">Дата и время выполнения компиляции в формате UTC.</param>
    public sealed record AnsAppCodeCompileResult(
        bool Success,
        int SourceFilesCount,
        int ExportedTypesCount,
        long ElapsedMilliseconds,
        IReadOnlyList<AnsCompilationError> Errors,
        DateTime CompiledAtUtc);



    /// <summary>
    /// Динамический рантайм-компилятор исходных <c>.cs</c>-файлов из общего каталога <c>App_Code</c>
    /// в единую коллекционную in-memory сборку с автоматическим отслеживанием изменений файлов 
    /// и прозрачной интеграцией с <see cref="AnsRazorViewCompiler"/>.
    /// </summary>
    public sealed partial class AnsAppCodeCompiler
        : IDisposable
    {

        private readonly IWebHostEnvironment _env;
        private readonly AnsMetadataReferenceProvider _referenceProvider;
        private readonly AnsRazorViewCompiler _viewCompiler;
        private readonly ILogger<AnsAppCodeCompiler> _logger;
        private readonly SemaphoreSlim _compileLock = new(1, 1);
        private readonly IDisposable? _watcherRegistration;
        private readonly string _appCodeRelativePath;

        private AssemblyLoadContext? _appCodeLoadContext;
        private AnsAppCodeCompileResult? _lastResult;
        private int _isDirty = 1;
        private bool _disposed;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AnsAppCodeCompiler"/> 
        /// и запускает отслеживание изменений <c>.cs</c>-файлов в каталоге <c>App_Code</c>.
        /// </summary>
        /// <param name="env">Среда выполнения веб-приложения.</param>
        /// <param name="options">Параметры конфигурации библиотеки <c>Ans.Net10.Web</c>.</param>
        /// <param name="referenceProvider">Провайдер метаданных сборок для компилятора Roslyn.</param>
        /// <param name="viewCompiler">Рантайм-компилятор Razor-представлений для сброса кэша при пересборке <c>App_Code</c>.</param>
        /// <param name="logger">Экземпляр логгера.</param>
        public AnsAppCodeCompiler(
            IWebHostEnvironment env,
            LibWebOptions options,
            AnsMetadataReferenceProvider referenceProvider,
            AnsRazorViewCompiler viewCompiler,
            ILogger<AnsAppCodeCompiler> logger)
        {
            ArgumentNullException.ThrowIfNull(env);
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(referenceProvider);
            ArgumentNullException.ThrowIfNull(viewCompiler);
            ArgumentNullException.ThrowIfNull(logger);

            _env = env;
            _referenceProvider = referenceProvider;
            _viewCompiler = viewCompiler;
            _logger = logger;

            _appCodeRelativePath = (options.DynamicCompilation?.AppCodePath ?? "App_Code")
                .Trim()
                .Trim('/')
                .Replace('\\', '/');

            var watchPattern1 = string.IsNullOrEmpty(_appCodeRelativePath)
                ? "**/*.cs"
                : $"{_appCodeRelativePath}/**/*.cs";

            _watcherRegistration = ChangeToken.OnChange(
                () => _env.ContentRootFileProvider.Watch(watchPattern1),
                _onSourceFilesChanged);
        }


        /* readonly properties */


        /// <summary>
        /// Возвращает результат последней выполненной компиляции каталога <c>App_Code</c>.
        /// </summary>
        public AnsAppCodeCompileResult? LastResult
            => _lastResult;


        /// <summary>
        /// Возвращает признак наличия нескомпилированных изменений в файлах каталога <c>App_Code</c>.
        /// </summary>
        public bool HasPendingChanges
            => Volatile.Read(ref _isDirty) == 1;


        /* methods */


        /// <summary>
        /// Проверяет наличие изменений в файлах <c>App_Code</c> и при необходимости 
        /// выполняет асинхронную пересборку перед компиляцией Razor-представлений.
        /// </summary>
        /// <param name="throwOnError">
        /// Если <see langword="true"/> и при компиляции возникли ошибки, 
        /// выбрасывает <see cref="AnsRazorCompilationException"/> с полным списком проблемных строк.
        /// </param>
        /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
        /// <returns>Задача, представляющая асинхронную операцию проверки и компиляции.</returns>
        public async ValueTask EnsureCompiledAsync(
            bool throwOnError = true,
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (Volatile.Read(ref _isDirty) == 0)
                return;

            var result1 = await CompileAsync(force: false, cancellationToken).ConfigureAwait(false);
            if (!result1.Success && throwOnError)
            {
                var firstFile1 = result1.Errors is [var firstError1, ..]
                    ? firstError1.FilePath
                    : $"/{_appCodeRelativePath}";

                string? sourceCode1 = null;
                var fullFirstFile1 = Path.Combine(
                    _env.ContentRootPath,
                    firstFile1.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(fullFirstFile1))
                    sourceCode1 = await _readFileWithRetryAsync(fullFirstFile1, cancellationToken)
                        .ConfigureAwait(false);

                throw new AnsRazorCompilationException(
                    firstFile1,
                    result1.Errors,
                    generatedCode: null,
                    sourceCode: sourceCode1);
            }
        }


        /// <summary>
        /// Асинхронно компилирует все <c>.cs</c>-файлы из каталога <c>App_Code</c> в единую сборку в памяти,
        /// обновляет ссылки в <see cref="AnsMetadataReferenceProvider"/> и сбрасывает кэш представлений.
        /// </summary>
        /// <param name="force">Если <see langword="true"/>, выполняет принудительную пересборку даже при отсутствии изменений.</param>
        /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
        /// <returns>Задача, результатом которой является отчет <see cref="AnsAppCodeCompileResult"/>.</returns>
        public async Task<AnsAppCodeCompileResult> CompileAsync(
            bool force = true,
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            await _compileLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (!force && Volatile.Read(ref _isDirty) == 0 && _lastResult is not null)
                    return _lastResult;

                Interlocked.Exchange(ref _isDirty, 0);
                var stopwatch1 = Stopwatch.StartNew();

                var fullDirPath1 = Path.GetFullPath(
                    Path.Combine(_env.ContentRootPath, _appCodeRelativePath));

                if (!Directory.Exists(fullDirPath1))
                {
                    _unloadPreviousAndClear();
                    stopwatch1.Stop();

                    var emptyResult1 = new AnsAppCodeCompileResult(
                        Success: true,
                        SourceFilesCount: 0,
                        ExportedTypesCount: 0,
                        ElapsedMilliseconds: stopwatch1.ElapsedMilliseconds,
                        Errors: [],
                        CompiledAtUtc: DateTime.UtcNow);

                    _lastResult = emptyResult1;
                    return emptyResult1;
                }

                var csFiles1 = Directory.GetFiles(fullDirPath1, "*.cs", SearchOption.AllDirectories)
                    .OrderBy(static f => f, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (csFiles1.Length == 0)
                {
                    _unloadPreviousAndClear();
                    stopwatch1.Stop();

                    var noFilesResult1 = new AnsAppCodeCompileResult(
                        Success: true,
                        SourceFilesCount: 0,
                        ExportedTypesCount: 0,
                        ElapsedMilliseconds: stopwatch1.ElapsedMilliseconds,
                        Errors: [],
                        CompiledAtUtc: DateTime.UtcNow);

                    _lastResult = noFilesResult1;
                    return noFilesResult1;
                }

                var parseOptions1 = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
                var syntaxTrees1 = new List<SyntaxTree>(csFiles1.Length);

                foreach (var filePath1 in csFiles1)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var code1 = await _readFileWithRetryAsync(filePath1, cancellationToken)
                        .ConfigureAwait(false);
                    syntaxTrees1.Add(CSharpSyntaxTree.ParseText(
                        code1,
                        parseOptions1,
                        path: filePath1,
                        encoding: Encoding.UTF8,
                        cancellationToken: cancellationToken));
                }

                var assemblyName1 = $"AnsAppCode_{Guid.NewGuid():N}";
                var compilationOptions1 = new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,
                    nullableContextOptions: NullableContextOptions.Enable);

                var compilation1 = CSharpCompilation.Create(
                    assemblyName1,
                    syntaxTrees1,
                    _referenceProvider.GetBaseReferences(),
                    compilationOptions1);

                var emitOptions1 = new EmitOptions(
                    debugInformationFormat: DebugInformationFormat.Embedded);

                using var peStream1 = new MemoryStream();
                var emitResult1 = compilation1.Emit(
                    peStream1,
                    options: emitOptions1,
                    cancellationToken: cancellationToken);

                if (!emitResult1.Success)
                {
                    var errors1 = emitResult1.Diagnostics
                        .Where(static x => x.IsWarningAsError || x.Severity == DiagnosticSeverity.Error)
                        .Select(x =>
                        {
                            var lineSpan1 = x.Location.GetMappedLineSpan();
                            var rawPath1 = string.IsNullOrEmpty(lineSpan1.Path)
                                ? _appCodeRelativePath
                                : Path.GetRelativePath(_env.ContentRootPath, lineSpan1.Path).Replace('\\', '/');
                            return new AnsCompilationError(
                                rawPath1,
                                lineSpan1.StartLinePosition.Line + 1,
                                lineSpan1.StartLinePosition.Character + 1,
                                x.Id,
                                x.GetMessage());
                        })
                        .ToArray();

                    stopwatch1.Stop();
                    _log.CompilationFailed(_logger, errors1.Length);

                    var failResult1 = new AnsAppCodeCompileResult(
                        Success: false,
                        SourceFilesCount: csFiles1.Length,
                        ExportedTypesCount: 0,
                        ElapsedMilliseconds: stopwatch1.ElapsedMilliseconds,
                        Errors: errors1,
                        CompiledAtUtc: DateTime.UtcNow);

                    _lastResult = failResult1;
                    return failResult1;
                }

                peStream1.Position = 0;
                var newContext1 = new AssemblyLoadContext($"AnsAppCodeCtx_{Guid.NewGuid():N}", isCollectible: true);
                var compiledAssembly1 = newContext1.LoadFromStream(peStream1);
                var exportedTypesCount1 = compiledAssembly1.GetExportedTypes().Length;

                peStream1.Position = 0;
                var metadataRef1 = MetadataReference.CreateFromStream(peStream1);

                var oldContext1 = _appCodeLoadContext;
                _appCodeLoadContext = newContext1;

                _referenceProvider.SetDynamicReferences([metadataRef1]);
                _viewCompiler.ResetCache();
                oldContext1?.Unload();

                stopwatch1.Stop();
                var successResult1 = new AnsAppCodeCompileResult(
                    Success: true,
                    SourceFilesCount: csFiles1.Length,
                    ExportedTypesCount: exportedTypesCount1,
                    ElapsedMilliseconds: stopwatch1.ElapsedMilliseconds,
                    Errors: [],
                    CompiledAtUtc: DateTime.UtcNow);

                _lastResult = successResult1;
                _log.CompilationSucceeded(_logger, csFiles1.Length, exportedTypesCount1, stopwatch1.ElapsedMilliseconds);
                return successResult1;
            }
            finally
            {
                _compileLock.Release();
            }
        }


        /// <summary>
        /// Освобождает ресурсы компилятора, отменяет подписку на изменения файлов и выгружает сборку <c>App_Code</c>.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _watcherRegistration?.Dispose();
            _compileLock.Dispose();
            _appCodeLoadContext?.Unload();
            _appCodeLoadContext = null;
        }


        /* privates */


        private void _onSourceFilesChanged()
        {
            Interlocked.Exchange(ref _isDirty, 1);
            _log.ChangesDetected(_logger, _appCodeRelativePath);
        }


        private void _unloadPreviousAndClear()
        {
            var oldContext1 = _appCodeLoadContext;
            _appCodeLoadContext = null;

            _referenceProvider.ClearDynamicReferences();
            _viewCompiler.ResetCache();
            oldContext1?.Unload();
        }


        private static async Task<string> _readFileWithRetryAsync(
            string filePath,
            CancellationToken cancellationToken)
        {
            const int maxAttempts1 = 4;
            for (var attempt1 = 1; ; attempt1++)
            {
                try
                {
                    return await File.ReadAllTextAsync(filePath, Encoding.UTF8, cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (IOException) when (attempt1 < maxAttempts1)
                {
                    await Task.Delay(50 * attempt1, cancellationToken).ConfigureAwait(false);
                }
            }
        }


        /* logs */


        private static partial class _log
        {
            [LoggerMessage(
                Level = LogLevel.Debug,
                Message = "[AnsAppCodeCompiler] Зафиксированы изменения исходных файлов в '{Path}'. Запланирована пересборка.")]
            public static partial void ChangesDetected(
                ILogger logger, string path);

            [LoggerMessage(
                Level = LogLevel.Error,
                Message = "[AnsAppCodeCompiler] Ошибка компиляции App_Code. Обнаружено ошибок: {ErrorsCount}. Предыдущая сборка сохранена.")]
            public static partial void CompilationFailed(
                ILogger logger, int errorsCount);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[AnsAppCodeCompiler] App_Code успешно скомпилирован за {ElapsedMs} мс. Файлов: {FilesCount}, экспортированных типов: {TypesCount}.")]
            public static partial void CompilationSucceeded(
                ILogger logger, int filesCount, int typesCount, long elapsedMs);
        }

    }

}
