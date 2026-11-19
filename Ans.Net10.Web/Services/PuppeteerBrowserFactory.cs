// rev 2026-11-19

using PuppeteerSharp;

namespace Ans.Net10.Web.Services
{

    /// <summary>
    /// Фабрика для управления жизненным циклом, очередью запросов и предоставления экземпляра браузера Puppeteer.
    /// </summary>
    public interface IBrowserFactory
    {
        /// <summary>
        /// Асинхронно возвращает активный экземпляр браузера.
        /// Если браузер ещё не запущен или его процесс был завершён, выполняет загрузку и запуск нового экземпляра.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены для прерывания ожидания инициализации браузера.</param>
        /// <returns>Задача, результатом которой является готовый к работе экземпляр <see cref="IBrowser"/>.</returns>
        /// <exception cref="ObjectDisposedException">Выбрасывается, если фабрика уже была освобождена.</exception>
        Task<IBrowser> GetBrowserAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Асинхронно резервирует слот для открытия новой вкладки в браузере, ограничивая число одновременных операций рендеринга.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены ожидания свободного слота.</param>
        /// <returns>Объект <see cref="IDisposable"/>, освобождающий занятый слот при вызове <see cref="IDisposable.Dispose"/>.</returns>
        Task<IDisposable> AcquirePageSlotAsync(CancellationToken cancellationToken = default);
    }



    /// <summary>
    /// Потокобезопасная реализация фабрики браузера <see cref="IBrowserFactory"/> на базе PuppeteerSharp.
    /// Поддерживает единый экземпляр браузера, ограничивает параллельные вкладки и автоматически пересоздаёт браузер при сбоях.
    /// </summary>
    public class PuppeteerBrowserFactory
        : IBrowserFactory,
        IAsyncDisposable
    {

        private readonly string _appPath;
        private readonly SemaphoreSlim _initSemaphore = new(1, 1);
        private readonly SemaphoreSlim _pageSlotSemaphore;
        private IBrowser? _browser;
        private string? _executablePath;
        private int _disposed;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PuppeteerBrowserFactory"/>.
        /// </summary>
        /// <param name="options">Параметры конфигурации сервиса PuppeteerSharp, содержащие путь для хранения файлов браузера.</param>
        public PuppeteerBrowserFactory(
            PuppeteerSharpServiceOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            var maxPages1 = Math.Max(1, options.MaxConcurrentPages);
            _appPath = options.AppPath;
            _pageSlotSemaphore = new SemaphoreSlim(maxPages1, maxPages1);
        }


        /* functions */


        /// <inheritdoc />
        public async Task<IDisposable> AcquirePageSlotAsync(
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
            await _pageSlotSemaphore.WaitAsync(cancellationToken);
            if (Volatile.Read(ref _disposed) != 0)
            {
                _pageSlotSemaphore.Release();
                throw new ObjectDisposedException(nameof(PuppeteerBrowserFactory));
            }
            return new PageSlotReleaser(_pageSlotSemaphore);
        }


        /// <inheritdoc />
        public async Task<IBrowser> GetBrowserAsync(
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);

            var currentBrowser1 = _browser;
            if (IsBrowserReady(currentBrowser1))
                return currentBrowser1!;

            await _initSemaphore.WaitAsync(cancellationToken);
            try
            {
                ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);

                if (IsBrowserReady(_browser))
                    return _browser!;
                _browser?.Dispose();
                _browser = null;

                if (string.IsNullOrEmpty(_executablePath) || !File.Exists(_executablePath))
                {
                    var browserFetcher1 = new BrowserFetcher(
                        new BrowserFetcherOptions
                        {
                            Path = _appPath
                        });

                    var installedBrowser1 = await browserFetcher1.DownloadAsync();
                    _executablePath = installedBrowser1.GetExecutablePath();
                }

                IBrowser browser1;
                try
                {
                    browser1 = await Puppeteer.LaunchAsync(new LaunchOptions
                    {
                        Headless = true,
                        ExecutablePath = _executablePath,
                        Args = [
                            "--no-sandbox",
                            "--disable-setuid-sandbox",
                            "--disable-dev-shm-usage",
                            "--disable-gpu",
                            "--disable-extensions",
                            "--no-first-run",
                            "--no-default-browser-check",
                            "--disable-background-networking",
                            "--mute-audio"]
                    });
                }
                catch
                {
                    _executablePath = null;
                    throw;
                }

                browser1.Disconnected += (_, _) =>
                {
                    if (ReferenceEquals(_browser, browser1))
                    {
                        _browser = null;
                        browser1.Dispose();
                    }
                };
                _browser = browser1;

                return _browser;
            }
            finally
            {
                _initSemaphore.Release();
            }
        }


        /* methods */


        /// <summary>
        /// Асинхронно закрывает запущенный экземпляр браузера и освобождает связанные с фабрикой ресурсы.
        /// </summary>
        /// <returns>Задача, представляющая асинхронную операцию освобождения ресурсов.</returns>
        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
                return;

            await _initSemaphore.WaitAsync();
            try
            {
                if (_browser != null)
                {
                    try
                    {
                        if (!_browser.IsClosed)
                            await _browser.CloseAsync();
                    }
                    catch
                    {
                        // Игнорируем ошибки закрытия при остановке сервиса.
                    }
                    finally
                    {
                        _browser.Dispose();
                        _browser = null;
                    }
                }
            }
            finally
            {
                _initSemaphore.Release();
            }

            GC.SuppressFinalize(this);
        }


        /* privates */


        private static bool IsBrowserReady(
            IBrowser? browser)
        {
            return browser is { IsClosed: false, IsConnected: true };
        }


        private sealed class PageSlotReleaser(
            SemaphoreSlim semaphore)
            : IDisposable
        {
            private int _released;

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _released, 1) == 0)
                    semaphore.Release();
            }
        }

    }

}
