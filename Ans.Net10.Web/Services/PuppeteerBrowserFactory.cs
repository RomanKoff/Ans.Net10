using PuppeteerSharp;

namespace Ans.Net10.Web.Services
{

    public interface IBrowserFactory
    {
        Task<IBrowser> GetBrowserAsync();
    }



    public class PuppeteerBrowserFactory
        : IBrowserFactory,
        IAsyncDisposable
    {

        private readonly string _appPath;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private IBrowser? _browser;


        /* ctor */


        public PuppeteerBrowserFactory(
            PuppeteerSharpServiceOptions options)
        {
            _appPath = options.AppPath;
        }


        /* functions */


        public async Task<IBrowser> GetBrowserAsync()
        {
            if (_browser != null && !_browser.IsClosed)
                return _browser;
            await _semaphore.WaitAsync();
            try
            {
                if (_browser != null && !_browser.IsClosed)
                    return _browser;
                var browserFetcher1 = new BrowserFetcher(
                    new BrowserFetcherOptions
                    {
                        Path = _appPath
                    });
                var installedBrowser1 = await browserFetcher1.DownloadAsync();
                _browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = installedBrowser1.GetExecutablePath(),
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
                return _browser;
            }
            finally
            {
                _semaphore.Release();
            }
        }


        /* methods */


        public async ValueTask DisposeAsync()
        {
            if (_browser != null && !_browser.IsClosed)
            {
                await _browser.CloseAsync();
                _browser.Dispose();
            }
            _semaphore.Dispose();
            GC.SuppressFinalize(this);
        }

    }

}
