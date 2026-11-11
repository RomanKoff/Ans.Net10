// rev 2026-11-10

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ans.Net10.Web.Nodes
{

    /// <summary>
    /// Фоновая служба автоматического мониторинга дискового пространства каталога Views/Nodes.
    /// Отслеживает создание, удаление и изменение файлов Razor-представлений (*.cshtml) и инициирует обновление карты маршрутов.
    /// </summary>
    public sealed partial class NodesWatcherService
        : BackgroundService
    {

        private readonly NodesMapper _mapper;
        private readonly ILogger<NodesWatcherService> _logger;
        private readonly FileSystemWatcher _watcher;
        private readonly SemaphoreSlim _signalSemaphore = new(0, 1);

        private int _hasChanges;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="NodesWatcherService"/>.
        /// </summary>
        /// <param name="mapper">Синглтон-исполнитель и хранитель карты Razor-представлений.</param>
        /// <param name="logger">Служба ведения фонового лога событий.</param>
        /// <exception cref="ArgumentNullException">Вызывается, если один из переданных параметров равен <see langword="null"/>.</exception>
        public NodesWatcherService(
            NodesMapper mapper,
            ILogger<NodesWatcherService> logger)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            ArgumentNullException.ThrowIfNull(logger);
            _mapper = mapper;
            _logger = logger;

            _watcher = new FileSystemWatcher
            {
                Path = _mapper.NodesPath,
                Filter = "*", // NodesMapper.VIEW_FILTER, 
                IncludeSubdirectories = true,
                NotifyFilter =
                    NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
            };

            _watcher.Created += _onViewsFileChanged;
            _watcher.Deleted += _onViewsFileChanged;
            _watcher.Renamed += _onViewsFileChanged;
        }


        /* methods */


        /// <summary>
        /// Запускает бесконечный фоновый цикл ожидания сигналов от файловой системы и координирует Debounce-затишье.
        /// </summary>
        /// <param name="stoppingToken">Токен остановки фонового процесса веб-сервера.</param>
        /// <returns>Поток-задача <see cref="Task"/> жизненного цикла службы.</returns>
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _log.WatcherStarted(_logger, _mapper.NodesPath);
            if (Directory.Exists(_mapper.NodesPath))
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
                    _log.RefreshTriggered(_logger);
                    await _mapper.InitializeAsync(stoppingToken).ConfigureAwait(false);
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
            _watcher.Created -= _onViewsFileChanged;
            _watcher.Deleted -= _onViewsFileChanged;
            _watcher.Renamed -= _onViewsFileChanged;
            _watcher.Dispose();
            _signalSemaphore.Dispose();
            base.Dispose();
        }


        /* privates */


        private void _onViewsFileChanged(
            object sender,
            FileSystemEventArgs e)
        {
            if (e.Name is null || e.Name.Contains('~'))
                return;

            if (!Directory.Exists(e.FullPath)
                && !e.Name.EndsWith(NodesMapper.VIEW_EXTENSION, StringComparison.OrdinalIgnoreCase))
                return;

            _log.FileChangeDetected(_logger, e.ChangeType, e.FullPath);

            Interlocked.Exchange(ref _hasChanges, 1);
            if (_signalSemaphore.CurrentCount == 0)
                _signalSemaphore.Release();
        }


        /* logs */


        private static partial class _log
        {
            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[NodesWatcherService] Мониторинг Razor-представлений запущен для каталога: '{Path}'")]
            public static partial void WatcherStarted(
                ILogger logger, string path);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[NodesWatcherService] Мониторинг Razor-представлений остановлен.")]
            public static partial void WatcherStopped(
                ILogger logger);

            [LoggerMessage(
                Level = LogLevel.Debug,
                Message = "[NodesWatcherService] Зафиксировано изменение файла ({ChangeType}): '{FullPath}'")]
            public static partial void FileChangeDetected(
                ILogger logger, WatcherChangeTypes changeType, string fullPath);

            [LoggerMessage(
                Level = LogLevel.Debug,
                Message = "[NodesWatcherService] Запущено окно ожидания затишья файловых операций (Debounce) для *.cshtml.")]
            public static partial void DebounceWindowStarted(
                ILogger logger);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[NodesWatcherService] Окно затишья успешно пройдено. Инициировано перестроение карты представлений.")]
            public static partial void RefreshTriggered(
                ILogger logger);
        }

    }

}
