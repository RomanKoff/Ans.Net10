// rev 2026-11-10

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace Ans.Net10.Web.Nodes
{

    /// <summary>
    /// Потокобезопасный исполнитель карты Razor-представлений динамических узлов.
    /// Обеспечивает мгновенное разрешение ЧПУ-путей за O(1) и атомарную подмену конфигурации.
    /// </summary>
    public sealed partial class NodesMapper
    {

        private readonly ILogger<NodesMapper> _logger;
        private readonly Lock _syncLock = new();

        private readonly int _indexPageLength = LibWebConsts.INDEX_PAGE_NAME.Length + 1;
        private readonly string _indexPageOfEnd = $"/{LibWebConsts.INDEX_PAGE_NAME}";
        private readonly int _indexNodeLength = LibWebConsts.INDEX_NODE_NAME.Length + 1;
        private readonly string _indexNodeOfStart = $"{LibWebConsts.INDEX_NODE_NAME}/";
        private readonly string _indexNodeOfEnd = $"/{LibWebConsts.INDEX_NODE_NAME}";


        /* consts */


        public const string NODES_FOLDER = "Views/Nodes";
        public const string VIEW_EXTENSION = ".cshtml";
        public const string VIEW_FILTER = $"*{VIEW_EXTENSION}";


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="NodesMapper"/>.
        /// </summary>
        /// <param name="env">Среда выполнения веб-приложения.</param>
        /// <param name="logger">Служба ведения системных логов.</param>
        /// <exception cref="ArgumentNullException">Инициируется, если переданные зависимости равны null.</exception>
        public NodesMapper(
            IWebHostEnvironment env,
            ILogger<NodesMapper> logger)
        {
            ArgumentNullException.ThrowIfNull(env);
            ArgumentNullException.ThrowIfNull(logger);
            NodesPath = Path.Combine(env.ContentRootPath, NODES_FOLDER).Replace('\\', '/');
            _logger = logger;
        }


        /* readonly properties */


        /// <summary>
        /// Возвращает абсолютный физический путь к корневому каталогу хранения Razor-представлений динамических узлов.
        /// </summary>
        /// <value>Строка канонического пути с нормализованными прямыми слэшами.</value>
        public string NodesPath { get; }


        /// <summary>
        /// Возвращает текущий неизменяемый снимок карты маршрутов, сопоставляющий ЧПУ-ключи с виртуальными путями к .cshtml файлам.
        /// </summary>
        /// <value>Потокобезопасный словарь <see cref="ImmutableDictionary{TKey, TValue}"/>.</value>
        public ImmutableDictionary<string, string> Map { get; private set; } = [];


        /* functions */


        /// <summary>
        /// Выполняет полное сканирование диска и перестроение карты представлений.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
        /// <returns>Значение <see langword="true"/>, если перестроение завершилось успехом; иначе — <see langword="false"/> (ошибка I/O).</returns>
        public async ValueTask<bool> InitializeAsync(
            CancellationToken cancellationToken = default)
        {
            _log.BuildStarted(_logger);

            if (!Directory.Exists(NodesPath))
            {
                _log.LogDirectoryNotFound(_logger, NodesPath);
                using (_syncLock.EnterScope())
                {
                    Map = [];
                }
                return true;
            }

            try
            {
                var newMap1 = new Dictionary<string, string>(StringComparer.Ordinal);
                string indexNodePath1 = Path.Combine(NodesPath, LibWebConsts.INDEX_NODE_NAME).Replace('\\', '/');

                // Шаг 1: Сканируем технический узел _main
                if (Directory.Exists(indexNodePath1))
                    _scanDirectory(indexNodePath1, newMap1, cancellationToken);

                // Шаг 2: Сканируем кастомные узлы
                var topDirectories1 = Directory.EnumerateDirectories(NodesPath);
                foreach (string dir1 in topDirectories1)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string normalizedDir1 = dir1.Replace('\\', '/');
                    if (normalizedDir1.EndsWith(_indexNodeOfEnd, StringComparison.OrdinalIgnoreCase))
                        continue;
                    _scanDirectory(normalizedDir1, newMap1, cancellationToken);
                }

                using (_syncLock.EnterScope())
                {
                    Map = newMap1.ToImmutableDictionary(StringComparer.Ordinal);
                }

                _log.BuildCompleted(_logger, Map.Count);
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                _log.LogRefreshError(_logger, ex.Message, ex);
                return false;
            }
        }


        /// <summary>
		/// Выполняет поиск виртуального пути к Razor-представлению по каноническому ключу запроса.
		/// </summary>
		/// <param name="requestPath">Нормализованный путь запроса пользователя (без ведущих/закрывающих слэшей).</param>
		/// <param name="viewPath">Выходной параметр, содержащий виртуальный путь вида <c>/Views/Nodes/...</c> в случае успеха.</param>
		/// <returns>Значение <see langword="true"/>, если представление найдено; иначе — <see langword="false"/>.</returns>
		public bool TryGetView(
            string requestPath,
            out string? viewPath)
        {
            string key1 = requestPath is not null
                ? requestPath.Trim('/').ToLowerInvariant()
                : string.Empty;
            return Map.TryGetValue(key1, out viewPath);
        }


        /* privates */


        private void _scanDirectory(
            string directoryPath,
            Dictionary<string, string> targetMap,
            CancellationToken cancellationToken)
        {
            var files1 = Directory.EnumerateFiles(directoryPath, VIEW_FILTER, SearchOption.AllDirectories);
            foreach (string file1 in files1)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string normalizedFilePath1 = file1.Replace('\\', '/');
                string relativePath1 = Path.GetRelativePath(NodesPath, normalizedFilePath1).Replace('\\', '/');
                string key1 = _extractCanonicalKey(relativePath1);
                string virtualPath1 = $"/{NODES_FOLDER}/{relativePath1}";
                targetMap[key1] = virtualPath1;
            }
        }


        private string _extractCanonicalKey(
            string relativePath)
        {
            string key1 = relativePath[..^VIEW_EXTENSION.Length].ToLowerInvariant();

            if (key1.EndsWith(_indexPageOfEnd, StringComparison.OrdinalIgnoreCase))
                key1 = key1[..^_indexPageLength];
            else if (key1.Equals(LibWebConsts.INDEX_PAGE_NAME, StringComparison.OrdinalIgnoreCase))
                key1 = string.Empty;

            if (key1.StartsWith(_indexNodeOfStart, StringComparison.OrdinalIgnoreCase))
                key1 = key1[_indexNodeLength..];
            else if (key1.Equals(LibWebConsts.INDEX_NODE_NAME, StringComparison.OrdinalIgnoreCase))
                key1 = string.Empty;

            return key1;
        }


        /* logs */


        private static partial class _log
        {
            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[NodesMapper] Запущено перестроение карты представлений.")]
            public static partial void BuildStarted(
                ILogger logger);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[NodesMapper] Перестроение карты представлений завершено успешно. Загружено элементов: {Count}.")]
            public static partial void BuildCompleted(
                ILogger logger, int count);

            [LoggerMessage(
                Level = LogLevel.Error,
                Message = "[NodesMapper] Ошибка перестроении карты представлений: {Message}")]
            public static partial void LogRefreshError(
                ILogger logger, string message, Exception ex);

            [LoggerMessage(
                Level = LogLevel.Warning,
                Message = "[NodesMapper] Корневой каталог узлов не найден: {RootPath}. Карта очищена.")]
            public static partial void LogDirectoryNotFound(
                ILogger logger, string rootPath);
        }

    }

}
