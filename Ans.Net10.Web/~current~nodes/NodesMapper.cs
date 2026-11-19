// rev 2026-11-19

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Frozen;

namespace Ans.Net10.Web
{

    /// <summary>
    /// Потокобезопасный исполнитель карты Razor-представлений динамических узлов.
    /// Обеспечивает мгновенное разрешение ЧПУ-путей за O(1) и атомарную подмену конфигурации.
    /// </summary>
    public sealed partial class NodesMapper
    {

        private readonly ILogger<NodesMapper> _logger;
        private readonly Lock _syncLock = new();

        private readonly int _indexPageLength = SuppNodes.INDEX_PAGE_NAME.Length + 1;
        private readonly string _indexPageOfEnd = $"/{SuppNodes.INDEX_PAGE_NAME}";
        private readonly int _indexNodeLength = SuppNodes.INDEX_NODE_NAME.Length + 1;
        private readonly string _indexNodeOfStart = $"{SuppNodes.INDEX_NODE_NAME}/";
        private readonly string _indexNodeOfEnd = $"/{SuppNodes.INDEX_NODE_NAME}";


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="NodesMapper"/>.
        /// </summary>
        /// <param name="env">Среда выполнения веб-приложения.</param>
        /// <param name="logger">Служба ведения системных логов.</param>
        /// <exception cref="ArgumentNullException">Инициируется, если переданные зависимости равны <see langword="null"/>.</exception>
        public NodesMapper(
            IWebHostEnvironment env,
            ILogger<NodesMapper> logger)
        {
            ArgumentNullException.ThrowIfNull(env);
            ArgumentNullException.ThrowIfNull(logger);
            NodesPath = Path.Combine(env.ContentRootPath, SuppNodes.NODES_ROOT).Replace('\\', '/');
            VirtualNodesRoot = $"/{SuppNodes.NODES_ROOT.Replace('\\', '/')}";
            _logger = logger;
        }


        /* readonly properties */


        /// <summary>
        /// Возвращает абсолютный физический путь к корневому каталогу хранения Razor-представлений динамических узлов.
        /// </summary>
        /// <value>Строка канонического пути с нормализованными прямыми слэшами.</value>
        public string NodesPath { get; }


        /// <summary>
        /// Возвращает базовый виртуальный путь к корневому каталогу представлений узлов (например, <c>"/Views/Nodes"</c>).
        /// </summary>
        /// <value>Строка виртуального пути с прямыми слэшами.</value>
        public string VirtualNodesRoot { get; }


        /// <summary>
        /// Возвращает текущий неизменяемый снимок карты маршрутов, сопоставляющий ЧПУ-ключи с виртуальными путями к <c>.cshtml</c> файлам.
        /// </summary>
        /// <value>Оптимизированный для чтения замороженный словарь <see cref="FrozenDictionary{TKey, TValue}"/>.</value>
        public FrozenDictionary<string, string> Map { get; private set; }
            = FrozenDictionary<string, string>.Empty;


        /* functions */


        /// <summary>
        /// Выполняет полное сканирование диска и перестроение карты представлений.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
        /// <returns>Значение <see langword="true"/>, если перестроение завершилось успехом; иначе — <see langword="false"/>.</returns>
        public ValueTask<bool> InitializeAsync(
            CancellationToken cancellationToken = default)
        {
            _log.BuildStarted(_logger);

            if (!Directory.Exists(NodesPath))
            {
                _log.DirectoryNotFound(_logger, NodesPath);
                using (_syncLock.EnterScope())
                {
                    Map = FrozenDictionary<string, string>.Empty;
                }
                return ValueTask.FromResult(true);
            }

            try
            {
                var newMap1 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string indexNodePath1 = $"{NodesPath}/{SuppNodes.INDEX_NODE_NAME}";

                // Шаг 1: Сканируем технический узел _main (приоритетные корневые маршруты)
                if (Directory.Exists(indexNodePath1))
                    _scanDirectory(indexNodePath1, newMap1, cancellationToken);

                // Шаг 2: Сканируем остальные узлы
                foreach (string dir1 in Directory.EnumerateDirectories(NodesPath))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string normalizedDir1 = dir1.Replace('\\', '/');
                    string dirName1 = Path.GetFileName(normalizedDir1);

                    if (dirName1.Contains('~')
                        || normalizedDir1.EndsWith(_indexNodeOfEnd, StringComparison.OrdinalIgnoreCase))
                        continue;

                    _scanDirectory(normalizedDir1, newMap1, cancellationToken);
                }

                using (_syncLock.EnterScope())
                {
                    Map = newMap1.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
                }

                _log.BuildCompleted(_logger, Map.Count);
                return ValueTask.FromResult(true);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                _log.RefreshError(_logger, ex.Message, ex);
                return ValueTask.FromResult(false);
            }
        }


        /// <summary>
        /// Выполняет поиск виртуального пути к Razor-представлению по каноническому ключу запроса.
        /// </summary>
        /// <param name="requestPath">Нормализованный путь запроса пользователя (без ведущих и завершающих слэшей).</param>
        /// <param name="viewPath">Выходной параметр, содержащий виртуальный путь вида <c>/Views/Nodes/...</c> в случае успеха.</param>
        /// <returns>Значение <see langword="true"/>, если представление найдено; иначе — <see langword="false"/>.</returns>
        public bool TryGetView(
            string? requestPath,
            out string? viewPath)
        {
            string key1 = requestPath is not null
                ? requestPath.Trim('/')
                : string.Empty;
            return Map.TryGetValue(key1, out viewPath);
        }



        /* privates */


        private void _scanDirectory(
            string directoryPath,
            Dictionary<string, string> targetMap,
            CancellationToken cancellationToken)
        {
            var files1 = Directory.EnumerateFiles(directoryPath, SuppNodes.VIEW_FILTER, SearchOption.AllDirectories);
            foreach (string file1 in files1)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string fileName1 = Path.GetFileName(file1);
                if (fileName1.StartsWith('_') || fileName1.Contains('~'))
                    continue;

                string normalizedFilePath1 = file1.Replace('\\', '/');
                string relativePath1 = Path.GetRelativePath(NodesPath, normalizedFilePath1).Replace('\\', '/');

                if (_hasIgnoredSubdirectory(relativePath1))
                    continue;

                string key1 = _extractCanonicalKey(relativePath1);
                string virtualPath1 = $"{VirtualNodesRoot}/{relativePath1}";
                targetMap.TryAdd(key1, virtualPath1);
            }
        }


        private static bool _hasIgnoredSubdirectory(
            string relativePath)
        {
            int slashIndex1 = relativePath.IndexOf('/');
            if (slashIndex1 < 0)
                return false;

            ReadOnlySpan<char> span1 = relativePath.AsSpan();
            int start1 = 0;

            while (true)
            {
                int nextSlash1 = span1[start1..].IndexOf('/');
                if (nextSlash1 < 0)
                    break;

                ReadOnlySpan<char> segment1 = span1.Slice(start1, nextSlash1);
                if (segment1.Contains('~'))
                    return true;

                start1 += nextSlash1 + 1;
            }

            return false;
        }


        private string _extractCanonicalKey(
            string relativePath)
        {
            string key1 = relativePath[..^SuppNodes.VIEW_EXTENSION.Length].ToLowerInvariant();

            if (key1.EndsWith(_indexPageOfEnd, StringComparison.OrdinalIgnoreCase))
                key1 = key1[..^_indexPageLength];
            else if (key1.Equals(SuppNodes.INDEX_PAGE_NAME, StringComparison.OrdinalIgnoreCase))
                key1 = string.Empty;

            if (key1.StartsWith(_indexNodeOfStart, StringComparison.OrdinalIgnoreCase))
                key1 = key1[_indexNodeLength..];
            else if (key1.Equals(SuppNodes.INDEX_NODE_NAME, StringComparison.OrdinalIgnoreCase))
                key1 = string.Empty;

            return key1;
        }


        /* logs */


        private static partial class _log
        {
            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[NodesMapper] Запущено перестроение NodesMapper.")]
            public static partial void BuildStarted(
                ILogger logger);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[NodesMapper] Перестроение NodesMapper завершено успешно. Загружено элементов: {count}.")]
            public static partial void BuildCompleted(
                ILogger logger, int count);

            [LoggerMessage(
                Level = LogLevel.Error,
                Message = "[NodesMapper] Ошибка при перестроении NodesMapper: {message}")]
            public static partial void RefreshError(
                ILogger logger, string message, Exception ex);

            [LoggerMessage(
                Level = LogLevel.Warning,
                Message = "[NodesMapper] Корневой каталог NodesMapper не найден: '{rootPath}'. Карта очищена.")]
            public static partial void DirectoryNotFound(
                ILogger logger, string rootPath);
        }

    }

}
