// rev 2026-11-10

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ans.Net10.Web
{

    /// <summary>
    /// Фоновое прерывание и служба автоматического мониторинга XML-файлов конфигурации карты сайта.
    /// Использует низкоуровневые уведомления файловой системы с задержкой (Debounce) для предотвращения дребезга.
    /// </summary>
    public sealed partial class SitemapWatcherService
        : BackgroundService
    {

        private readonly SitemapBuilder _builder;
        private readonly ILogger<SitemapWatcherService> _logger;
        private readonly FileSystemWatcher _watcher;
        private readonly SemaphoreSlim _signalSemaphore = new(0, 1);

        private int _hasChanges;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SitemapWatcherService"/>.
        /// </summary>
        /// <param name="builder">Синглтон-строитель графа навигации, подлежащий актуализации.</param>
        /// <param name="logger">Экземпляр логгера фоновой службы.</param>
        /// <exception cref="ArgumentNullException">Вызывается, если один из параметров равен <see langword="null"/>.</exception>
        public SitemapWatcherService(
            SitemapBuilder builder,
            ILogger<SitemapWatcherService> logger)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(logger);
            _builder = builder;
            _logger = logger;

            _watcher = new FileSystemWatcher
            {
                Path = _builder.SitemapPath,
                Filter = "*", // "_map-*.xml",
                IncludeSubdirectories = true,
                NotifyFilter =
                    NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.LastWrite
                    | NotifyFilters.Size
            };

            _watcher.Changed += _onSitemapFileChanged;
            _watcher.Created += _onSitemapFileChanged;
            _watcher.Deleted += _onSitemapFileChanged;
            _watcher.Renamed += _onSitemapFileChanged;
        }


        /* methods */


        /// <summary>
        /// Выполняет основную асинхронную логику фонового ожидания сигналов и перестроения карты сайта.
        /// </summary>
        /// <param name="stoppingToken">Токен остановки фоновой службы.</param>
        /// <returns>Задание <see cref="Task"/>, представляющее жизненный цикл фоновой службы.</returns>
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _log.WatcherStarted(_logger, _builder.SitemapPath);
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
                    _log.RebuildTriggered(_logger);
                    await _builder.InitializeAsync(stoppingToken).ConfigureAwait(false);
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
            _watcher.Changed -= _onSitemapFileChanged;
            _watcher.Created -= _onSitemapFileChanged;
            _watcher.Deleted -= _onSitemapFileChanged;
            _watcher.Renamed -= _onSitemapFileChanged;
            _watcher.Dispose();
            _signalSemaphore.Dispose();
            base.Dispose();
        }


        /* privates */


        private void _onSitemapFileChanged(
            object sender,
            FileSystemEventArgs e)
        {
            if (e.Name is null)
                return;

            if (!Directory.Exists(e.FullPath))
            {
                string fileName1 = Path.GetFileName(e.Name);
                if (!fileName1.StartsWith("_map-", StringComparison.OrdinalIgnoreCase)
                    || !fileName1.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    return;
            }

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
                Message = "[SitemapWatcherService] Мониторинг карты сайта запущен для каталога: '{path}'.")]
            public static partial void WatcherStarted(
                ILogger logger, string path);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[SitemapWatcherService] Мониторинг карты сайта остановлен.")]
            public static partial void WatcherStopped(
                ILogger logger);

            [LoggerMessage(
                Level = LogLevel.Debug,
                Message = "[SitemapWatcherService] Зафиксировано изменение файла ({changeType}): '{fullPath}'.")]
            public static partial void FileChangeDetected(
                ILogger logger, WatcherChangeTypes changeType, string fullPath);

            [LoggerMessage(
                Level = LogLevel.Debug,
                Message = "[SitemapWatcherService] Запущено окно ожидания затишья файловых операций (Debounce) для _map-*.xml.")]
            public static partial void DebounceWindowStarted(
                ILogger logger);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[SitemapWatcherService] Окно затишья успешно пройдено. Инициировано перестроение карты сайта.")]
            public static partial void RebuildTriggered(
                ILogger logger);

            [LoggerMessage(
                Level = LogLevel.Error,
                Message = "[SitemapWatcherService] Ошибка перестроения карты сайта: {message}")]
            public static partial void RebuildError(
                ILogger logger, string message, Exception ex);
        }

    }

}
