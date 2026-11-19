// rev 2026-11-19

using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Ans.Net10.Web.Services
{

    /*
     
        var pdfBytes1 = await _pdfService.GeneratePdfFromViewAsync(
            "Default/InventorySheet",
            sheet1,
            PdfPresets.A4LandscapeWithPageNumbers(),
            cancellationToken: cancellationToken);

     */



    /// <summary>
    /// Сервис для генерации PDF-документов из HTML или Razor-представлений с помощью PuppeteerSharp.
    /// </summary>
    public interface IPdfService
    {
        /// <summary>
        /// Асинхронно генерирует PDF-документ из готовой HTML-разметки.
        /// </summary>
        /// <param name="htmlContent">Исходный HTML-код страницы.</param>
        /// <param name="options">Параметры формирования PDF. Если не заданы, используется <see cref="PdfPresets.A4Portrait"/>.</param>
        /// <param name="allowExternalRequests">Разрешить ли загрузку внешних ресурсов (шрифтов, стилей, изображений) по сети.</param>
        /// <param name="renderTimeoutMs">Таймаут загрузки контента страницы в миллисекундах (по умолчанию 30 000 мс).</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Массив байтов сгенерированного PDF-файла.</returns>
        Task<byte[]> GeneratePdfFromHtmlAsync(
            string htmlContent,
            PdfOptions? options = null,
            bool allowExternalRequests = false,
            int renderTimeoutMs = 30_000,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Асинхронно рендерит Razor-представление в HTML и конвертирует его в PDF-документ.
        /// </summary>
        /// <typeparam name="TModel">Тип модели представления.</typeparam>
        /// <param name="viewName">Путь или имя Razor-представления.</param>
        /// <param name="model">Модель данных для рендеринга.</param>
        /// <param name="options">Параметры формирования PDF. Если не заданы, используется <see cref="PdfPresets.A4Portrait"/>.</param>
        /// <param name="allowExternalRequests">Разрешить ли загрузку внешних ресурсов (шрифтов, стилей, изображений) по сети.</param>
        /// <param name="renderTimeoutMs">Таймаут загрузки контента страницы в миллисекундах (по умолчанию 30 000 мс).</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Массив байтов сгенерированного PDF-файла.</returns>
        Task<byte[]> GeneratePdfFromViewAsync<TModel>(
            string viewName,
            TModel model,
            PdfOptions? options = null,
            bool allowExternalRequests = false,
            int renderTimeoutMs = 30_000,
            CancellationToken cancellationToken = default);
    }



    /// <summary>
    /// Реализация сервиса генерации PDF <see cref="IPdfService"/> на базе <see cref="IBrowserFactory"/> и <see cref="IViewRenderService"/>.
    /// </summary>
    public class AnsPdfService
        : IPdfService
    {

        private readonly IBrowserFactory _browserFactory;
        private readonly IViewRenderService _viewRenderService;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AnsPdfService"/>.
        /// </summary>
        /// <param name="browserFactory">Фабрика экземпляров браузера PuppeteerSharp.</param>
        /// <param name="viewRenderService">Сервис рендеринга Razor-представлений в строку.</param>
        public AnsPdfService(
            IBrowserFactory browserFactory,
            IViewRenderService viewRenderService)
        {
            _browserFactory = browserFactory;
            _viewRenderService = viewRenderService;
        }


        /* functions */


        /// <inheritdoc />
        public async Task<byte[]> GeneratePdfFromHtmlAsync(
            string htmlContent,
            PdfOptions? options = null,
            bool allowExternalRequests = false,
            int renderTimeoutMs = 30_000,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(htmlContent);

            using var pageSlot1 = await _browserFactory.AcquirePageSlotAsync(cancellationToken);

            var browser1 = await _browserFactory.GetBrowserAsync(cancellationToken);
            await using var page1 = await browser1.NewPageAsync();

            try
            {
                await page1.SetJavaScriptEnabledAsync(false);

                if (!allowExternalRequests)
                {
                    await page1.SetRequestInterceptionAsync(true);
                    page1.Request += async (_, e) =>
                    {
                        if (e.Request.Url.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
                            || e.Request.Url.StartsWith("about:", StringComparison.OrdinalIgnoreCase))
                        {
                            await e.Request.ContinueAsync();
                        }
                        else
                        {
                            await e.Request.AbortAsync();
                        }
                    };
                }

                await page1.SetContentAsync(htmlContent, new SetContentOptions
                {
                    WaitUntil = [WaitUntilNavigation.Load],
                    Timeout = renderTimeoutMs
                });

                var pdfOptions1 = options ?? PdfPresets.A4Portrait();
                return await page1.PdfDataAsync(pdfOptions1);
            }
            finally
            {
                if (!page1.IsClosed)
                    await page1.CloseAsync();
            }
        }


        /// <inheritdoc />
        public async Task<byte[]> GeneratePdfFromViewAsync<TModel>(
            string viewName,
            TModel model,
            PdfOptions? options = null,
            bool allowExternalRequests = false,
            int renderTimeoutMs = 30_000,
            CancellationToken cancellationToken = default)
        {
            var htmlContent1 = await _viewRenderService.RenderViewToStringAsync(viewName, model);
            return await GeneratePdfFromHtmlAsync(
                htmlContent1,
                options,
                allowExternalRequests,
                renderTimeoutMs,
                cancellationToken);
        }

    }



    /// <summary>
    /// Предоставляет готовые наборы параметров <see cref="PdfOptions"/> для типовых сценариев печати документов.
    /// </summary>
    public static class PdfPresets
    {

        /// <summary>
        /// Стандартный HTML-шаблон нижнего колонтитула с нумерацией страниц.
        /// </summary>
        public const string DEFAULT_FOOTER_TEMPLATE = @"
<div style=""width: 100%; padding: 0 10mm; font-family: 'Segoe UI', Arial, sans-serif; font-size: 11px; text-align: right;"">
    <div>Страница <span class=""pageNumber""></span> из <span class=""totalPages""></span></div>
</div>";


        /* functions */


        /// <summary>
        /// Возвращает настройки для книжной ориентации A4 без колонтитулов.
        /// </summary>
        /// <param name="margin">Отступы со всех сторон (по умолчанию 10mm).</param>
        /// <returns>Экземпляр <see cref="PdfOptions"/>.</returns>
        public static PdfOptions A4Portrait(
            string margin = "10mm")
        {
            return CreateBaseOptions(
                landscape: false,
                displayHeaderFooter: false,
                margin: margin);
        }


        /// <summary>
        /// Возвращает настройки для книжной ориентации A4 со стандартным нижним колонтитулом нумерации страниц.
        /// </summary>
        /// <param name="margin">Отступы со всех сторон (по умолчанию 10mm).</param>
        /// <param name="footerTemplate">Пользовательский HTML-шаблон футера. Если не задан, используется стандартный.</param>
        /// <returns>Экземпляр <see cref="PdfOptions"/>.</returns>
        public static PdfOptions A4PortraitWithPageNumbers(
            string margin = "10mm",
            string? footerTemplate = null)
        {
            return CreateBaseOptions(
                landscape: false,
                displayHeaderFooter: true,
                margin: margin,
                footerTemplate: footerTemplate ?? DEFAULT_FOOTER_TEMPLATE);
        }


        /// <summary>
        /// Возвращает настройки для альбомной ориентации A4 без колонтитулов.
        /// </summary>
        /// <param name="margin">Отступы со всех сторон (по умолчанию 10mm).</param>
        /// <returns>Экземпляр <see cref="PdfOptions"/>.</returns>
        public static PdfOptions A4Landscape(
            string margin = "10mm")
        {
            return CreateBaseOptions(
                landscape: true,
                displayHeaderFooter: false,
                margin: margin);
        }


        /// <summary>
        /// Возвращает настройки для альбомной ориентации A4 со стандартным нижним колонтитулом нумерации страниц.
        /// </summary>
        /// <param name="margin">Отступы со всех сторон (по умолчанию 10mm).</param>
        /// <param name="footerTemplate">Пользовательский HTML-шаблон футера. Если не задан, используется стандартный.</param>
        /// <returns>Экземпляр <see cref="PdfOptions"/>.</returns>
        public static PdfOptions A4LandscapeWithPageNumbers(
            string margin = "10mm",
            string? footerTemplate = null)
        {
            return CreateBaseOptions(
                landscape: true,
                displayHeaderFooter: true,
                margin: margin,
                footerTemplate: footerTemplate ?? DEFAULT_FOOTER_TEMPLATE);
        }


        /* privates */


        private static PdfOptions CreateBaseOptions(
            bool landscape,
            bool displayHeaderFooter,
            string margin,
            string? footerTemplate = null)
        {
            return new PdfOptions
            {
                Format = PaperFormat.A4,
                Landscape = landscape,
                PrintBackground = true,
                DisplayHeaderFooter = displayHeaderFooter,
                PreferCSSPageSize = true,
                MarginOptions = new MarginOptions
                {
                    Top = margin,
                    Bottom = margin,
                    Left = margin,
                    Right = margin
                },
                HeaderTemplate = displayHeaderFooter ? " " : null,
                FooterTemplate = footerTemplate
            };
        }

    }

}
