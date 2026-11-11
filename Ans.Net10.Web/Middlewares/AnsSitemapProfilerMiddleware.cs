using Ans.Net10.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ans.Net10.Web.Middlewares
{

    /// <summary>
    /// Инфраструктурный компонент конвейера (Middleware), обеспечивающий сопряжение данных 
    /// проиндексированной карты сайта с Триадой профилей оркеструющего контекста <see cref="CurrentContext"/>.
    /// </summary>
    public sealed partial class AnsSitemapProfilerMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<AnsSitemapProfilerMiddleware> _logger;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AnsSitemapProfilerMiddleware"/>.
        /// </summary>
        /// <param name="next">Делегат, представляющий следующую функцию обработки в конвейере HTTP.</param>
        /// <param name="logger">Служба ведения системных логов.</param>
        /// <exception cref="ArgumentNullException">Вызывается, если один из входящих параметров равен <see langword="null"/>.</exception>
        public AnsSitemapProfilerMiddleware(
            RequestDelegate next,
            ILogger<AnsSitemapProfilerMiddleware> logger)
        {
            ArgumentNullException.ThrowIfNull(next);
            ArgumentNullException.ThrowIfNull(logger);
            _next = next;
            _logger = logger;
        }


        /* methods */


        /// <summary>
        /// Выполняет перехват канонического URL-адреса запроса, извлекает метаданные из индекса 
        /// карты сайта O(1) и наполняет Триаду профилей текущего контекста.
        /// </summary>
        /// <param name="context">Контекст текущего обрабатываемого HTTP-запроса.</param>
        /// <param name="current">Оркестровый контекст текущего запроса (Scoped-сервис).</param>
        /// <returns>Поток-задача асинхронного выполнения операции конвейера.</returns>
        /// <exception cref="ArgumentNullException">Вызывается, если <paramref name="context"/> или <paramref name="current"/> равен <see langword="null"/>.</exception>
        public async Task InvokeAsync(
            HttpContext context,
            CurrentContext current)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(current);

            var requestPath1 = current.Request.RelativeUrl;

            current.Site.Title = current.Options.AppTitle ?? SuppApp.CallingAssembly?.Name ?? string.Empty;

            var pageMapItem1 = current.Sitemap.GetPage(requestPath1);

            if (pageMapItem1 is not null)
            {
                current.Page.MapItem = pageMapItem1;
                current.Node.MapItem = pageMapItem1.Node;
                _log.SitemapContextMatched(_logger, requestPath1);
            }
            else
            {
                _log.SitemapContextNotFound(_logger, requestPath1);
            }

            await _next(context);
        }


        /* logs */


        private static partial class _log
        {
            [LoggerMessage(
                Level = LogLevel.Debug,
                Message = "[Ans.Net10.Web] Триада профилей метаданных успешно заполнена из карты сайта по ключу: {CombinedKey}")]
            public static partial void SitemapContextMatched(ILogger logger, string combinedKey);

            [LoggerMessage(
                Level = LogLevel.Debug,
                Message = "[Ans.Net10.Web] Запрос по ключу '{CombinedKey}' отсутствует в карте сайта. Метаданные триады не заполнены.")]
            public static partial void SitemapContextNotFound(ILogger logger, string combinedKey);
        }

    }

}
