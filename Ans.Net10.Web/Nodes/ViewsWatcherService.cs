// rev 2026-11-18

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ans.Net10.Web.Nodes
{

    /// <summary>
    /// Единая фоновая служба мониторинга изменений в каталоге представлений <c>Views</c>.
    /// Распределяет события файловой системы по целевым триггерам (перестройка карты сайта и карты представлений) с окном стабилизации (Debounce).
    /// </summary>
    public sealed partial class ViewsWatcherService
        : BackgroundService
    {

        private readonly string _viewsPath;
        private readonly SitemapBuilder _sitemapBuilder;
        private readonly NodesMapper _nodesMapper;
        private readonly ILogger<ViewsWatcherService> _logger;
        private readonly FileSystemWatcher? _watcher;
        private readonly SemaphoreSlim _signalSemaphore = new(0, 1);

        private int _hasChanges;
        private int _hasSitemapChanges;
        private int _hasViewsChanges;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ViewsWatcherService"/>.
        /// </summary>
        /// <param name="env">Окружение веб-приложения для определения пути к каталогу представлений.</param>
        /// <param name="sitemapBuilder">Синглтон-строитель графа навигации, подлежащий актуализации.</param>
        /// <param name="nodesMapper">Синглтон-исполнитель карты Razor-представлений, подлежащий актуализации.</param>
        /// <param name="logger">Экземпляр логгера фоновой службы.</param>
        /// <exception cref="ArgumentNullException">Вызывается, если один из параметров равен <see langword="null"/>.</exception>
        public ViewsWatcherService(
            IWebHostEnvironment env,
            SitemapBuilder sitemapBuilder,
            NodesMapper nodesMapper,
            ILogger<ViewsWatcherService> logger)
        {
            ArgumentNullException.ThrowIfNull(env);
            ArgumentNullException.ThrowIfNull(sitemapBuilder);
            ArgumentNullException.ThrowIfNull(nodesMapper);
            ArgumentNullException.ThrowIfNull(logger);
            _sitemapBuilder = sitemapBuilder;
            _nodesMapper = nodesMapper;
            _logger = logger;
            _viewsPath = Path.Combine(env.ContentRootPath, SuppNodes.VIEWS_ROOT);

            if (!Directory.Exists(_viewsPath))
                return;

            _watcher = new FileSystemWatcher
            {
                Path = _viewsPath,
                Filter = "*",
                IncludeSubdirectories = true,
                NotifyFilter =
                    NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.LastWrite
                    | NotifyFilters.Size
            };

            _watcher.Changed += _onViewsChanged;
            _watcher.Created += _onViewsChanged;
            _watcher.Deleted += _onViewsChanged;
            _watcher.Renamed += _onViewsChanged;
        }


        /* methods */


        /// <summary>
        /// Выполняет основную асинхронную логику фонового ожидания сигналов и запуска целевых обработчиков изменений.
        /// </summary>
        /// <param name="stoppingToken">Токен остановки фоновой службы.</param>
        /// <returns>Задание <see cref="Task"/>, представляющее жизненный цикл фоновой службы.</returns>
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            if (_watcher is null)
            {
                _log.DirectoryNotFound(_logger, _viewsPath);
                return;
            }

            _log.WatcherStarted(_logger, _viewsPath);
            _watcher.EnableRaisingEvents = true;
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await _signalSemaphore.WaitAsync(stoppingToken).ConfigureAwait(false);
                    while (Interlocked.CompareExchange(ref _hasChanges, 0, 1) == 1)
                    {
                        _log.DebounceWindowStarted(_logger);
                        await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
                    }

                    if (Interlocked.CompareExchange(ref _hasSitemapChanges, 0, 1) == 1)
                    {
                        _log.SitemapRebuildTriggered(_logger);
                        await _sitemapBuilder.InitializeAsync(stoppingToken).ConfigureAwait(false);
                    }

                    if (Interlocked.CompareExchange(ref _hasViewsChanges, 0, 1) == 1)
                    {
                        _log.ViewsRebuildTriggered(_logger);
                        await _nodesMapper.InitializeAsync(stoppingToken).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _log.WatcherStopped(_logger);
            }
            finally
            {
                _watcher.EnableRaisingEvents = false;
            }
        }


        /// <inheritdoc />
        public override void Dispose()
        {
            if (_watcher is not null)
            {
                _watcher.Changed -= _onViewsChanged;
                _watcher.Created -= _onViewsChanged;
                _watcher.Deleted -= _onViewsChanged;
                _watcher.Renamed -= _onViewsChanged;
                _watcher.Dispose();
            }
            _signalSemaphore.Dispose();
            base.Dispose();
        }


        /* privates */


        private void _onViewsChanged(
            object sender,
            FileSystemEventArgs e)
        {
            if (e.Name is null)
                return;

            bool triggered1 = _evaluateTriggers(e.ChangeType, e.FullPath, e.Name);
            if (e is RenamedEventArgs renamed1 && renamed1.OldName is not null)
                triggered1 |= _evaluateTriggers(e.ChangeType, renamed1.OldFullPath, renamed1.OldName);

            if (!triggered1)
                return;

            _log.FileChangeDetected(_logger, e.ChangeType, e.FullPath);

            Interlocked.Exchange(ref _hasChanges, 1);
            if (_signalSemaphore.CurrentCount == 0)
                _signalSemaphore.Release();
        }


        private bool _evaluateTriggers(
            WatcherChangeTypes changeType,
            string fullPath,
            string relativeToViewsName)
        {
            bool triggered1 = false;

            // sitemap trigger
            if (_isSitemapTarget(changeType, fullPath, relativeToViewsName))
            {
                Interlocked.Exchange(ref _hasSitemapChanges, 1);
                triggered1 = true;
            }

            // nodes views map trigger
            if (_isNodesViewTarget(changeType, fullPath, relativeToViewsName))
            {
                Interlocked.Exchange(ref _hasViewsChanges, 1);
                triggered1 = true;
            }

            return triggered1;
        }


        private bool _isSitemapTarget(
            WatcherChangeTypes changeType,
            string fullPath,
            string relativeToViewsName)
        {
            if (!relativeToViewsName.StartsWith(SuppNodes.NODES_SUBDIR_PREFIX_SLASH, StringComparison.OrdinalIgnoreCase))
                return false;

            if (relativeToViewsName.Contains('~'))
                return false;

            string relativeToNodes1 = relativeToViewsName[SuppNodes.NODES_SUBDIR_PREFIX_SLASH.Length..];
            string? directoryName1 = Path.GetDirectoryName(relativeToNodes1);
            string fileName1 = Path.GetFileName(relativeToNodes1);

            if (string.IsNullOrEmpty(fileName1))
                return false;

            // 1. Корневой уровень Views/Nodes
            if (string.IsNullOrEmpty(directoryName1))
            {
                if (fileName1.EndsWith(SuppNodes.MAP_EXTENSION, StringComparison.OrdinalIgnoreCase))
                    return string.Equals(fileName1, SuppNodes.MAPNODES_FILENAME, StringComparison.OrdinalIgnoreCase);

                if (changeType is WatcherChangeTypes.Deleted or WatcherChangeTypes.Renamed)
                    return _sitemapBuilder.NodesIndex.ContainsKey(fileName1);

                if (Directory.Exists(fullPath))
                    return File.Exists(Path.Combine(fullPath, SuppNodes.MAPPAGES_FILENAME));

                return false;
            }

            // 2. Первый уровень вложенности Views/Nodes/{nodeName}/_map-pages.xml
            bool isFirstLevelSubdir1 =
                !directoryName1.Contains(Path.DirectorySeparatorChar)
                && !directoryName1.Contains(Path.AltDirectorySeparatorChar);

            return isFirstLevelSubdir1
                && fileName1.EndsWith(SuppNodes.MAP_EXTENSION, StringComparison.OrdinalIgnoreCase)
                && string.Equals(fileName1, SuppNodes.MAPPAGES_FILENAME, StringComparison.OrdinalIgnoreCase);
        }


        private static bool _isNodesViewTarget(
            WatcherChangeTypes changeType,
            string fullPath,
            string relativeToViewsName)
        {
            if (changeType is WatcherChangeTypes.Changed)
                return false;

            if (!relativeToViewsName.StartsWith(SuppNodes.NODES_SUBDIR_PREFIX_SLASH, StringComparison.OrdinalIgnoreCase))
                return false;

            if (relativeToViewsName.Contains('~'))
                return false;

            string fileName1 = Path.GetFileName(relativeToViewsName);
            if (string.IsNullOrEmpty(fileName1))
                return false;

            if (fileName1.EndsWith(SuppNodes.VIEW_EXTENSION, StringComparison.OrdinalIgnoreCase))
                return !fileName1.StartsWith('_');

            if (Path.HasExtension(fileName1))
                return false;

            return Directory.Exists(fullPath)
                || changeType is WatcherChangeTypes.Deleted or WatcherChangeTypes.Renamed;
        }


        /* logs */


        private static partial class _log
        {
            [LoggerMessage(
                Level = LogLevel.Warning,
                Message = "[ViewsWatcherService] Каталог Views не найден по пути: '{path}'. Мониторинг отключен.")]
            public static partial void DirectoryNotFound(
                ILogger logger, string path);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[ViewsWatcherService] Мониторинг Views запущен для каталога: '{path}'.")]
            public static partial void WatcherStarted(
                ILogger logger, string path);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[ViewsWatcherService] Мониторинг Views остановлен.")]
            public static partial void WatcherStopped(
                ILogger logger);

            [LoggerMessage(
                Level = LogLevel.Debug,
                Message = "[ViewsWatcherService] Зафиксировано релевантное изменение ({changeType}): '{fullPath}'.")]
            public static partial void FileChangeDetected(
                ILogger logger, WatcherChangeTypes changeType, string fullPath);

            [LoggerMessage(
                Level = LogLevel.Debug,
                Message = "[ViewsWatcherService] Запущено окно ожидания затишья файловых операций (Debounce).")]
            public static partial void DebounceWindowStarted(
                ILogger logger);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[ViewsWatcherService] Окно затишья пройдено. Инициировано перестроение SitemapBuilder.")]
            public static partial void SitemapRebuildTriggered(
                ILogger logger);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[ViewsWatcherService] Окно затишья пройдено. Инициировано перестроение NodesMapper.")]
            public static partial void ViewsRebuildTriggered(
                ILogger logger);
        }

    }

}
