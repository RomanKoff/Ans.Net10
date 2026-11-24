// rev 2026-11-22

using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Ans.Net10.Web.Compilation
{

    /// <summary>
    /// Потокобезопасный провайдер метаданных сборок (<see cref="MetadataReference"/>), 
    /// обеспечивающий компилятор Roslyn ссылками на базовые библиотеки платформы и динамические сборки моделей.
    /// </summary>
    public sealed class AnsMetadataReferenceProvider
    {

        private readonly Lock _syncLock = new();
        private readonly Lazy<ImmutableArray<MetadataReference>> _baseReferences;
        private ImmutableArray<MetadataReference> _dynamicReferences = [];


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AnsMetadataReferenceProvider"/>.
        /// </summary>
        public AnsMetadataReferenceProvider()
        {
            _baseReferences = new Lazy<ImmutableArray<MetadataReference>>(
                _buildBaseReferences,
                LazyThreadSafetyMode.ExecutionAndPublication);
        }


        /* functions */


        /// <summary>
        /// Возвращает полный актуальный набор ссылок на метаданные сборок, 
        /// объединяющий базовые сборки приложения и текущие динамические сборки моделей.
        /// </summary>
        /// <returns>Неизменяемый массив ссылок <see cref="MetadataReference"/> для компиляции.</returns>
        public ImmutableArray<MetadataReference> GetAllReferences()
        {
            lock (_syncLock)
            {
                return _dynamicReferences.IsEmpty
                    ? _baseReferences.Value
                    : [.. _baseReferences.Value, .. _dynamicReferences];
            }
        }


        /// <summary>
        /// Возвращает только базовые ссылки на метаданные стандартных и подключенных при старте сборок.
        /// </summary>
        /// <returns>Неизменяемый массив базовых ссылок <see cref="MetadataReference"/>.</returns>
        public ImmutableArray<MetadataReference> GetBaseReferences()
        {
            return _baseReferences.Value;
        }


        /// <summary>
        /// Атомарно обновляет набор метаданных для динамически загружаемых сборок моделей.
        /// </summary>
        /// <param name="references">Новая коллекция ссылок <see cref="MetadataReference"/> динамических сборок.</param>
        public void SetDynamicReferences(
            IEnumerable<MetadataReference> references)
        {
            ArgumentNullException.ThrowIfNull(references);

            var snapshot1 = references.ToImmutableArray();
            lock (_syncLock)
            {
                _dynamicReferences = snapshot1;
            }
        }


        /* methods */


        /// <summary>
        /// Очищает ссылки на динамически загруженные сборки моделей.
        /// </summary>
        public void ClearDynamicReferences()
        {
            lock (_syncLock)
            {
                _dynamicReferences = [];
            }
        }


        /* privates */


        private static ImmutableArray<MetadataReference> _buildBaseReferences()
        {
            var references1 = new List<MetadataReference>();
            var loadedPaths1 = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string tpa1
                && !string.IsNullOrEmpty(tpa1))
                foreach (var path1 in tpa1.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
                    if (File.Exists(path1)
                        && loadedPaths1.Add(path1))
                        references1.Add(MetadataReference.CreateFromFile(path1));

            foreach (var assembly1 in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly1.IsDynamic
                    || string.IsNullOrEmpty(assembly1.Location))
                    continue;
                if (File.Exists(assembly1.Location)
                    && loadedPaths1.Add(assembly1.Location))
                    references1.Add(MetadataReference.CreateFromFile(assembly1.Location));
            }

            return [.. references1];
        }

    }
    
}
