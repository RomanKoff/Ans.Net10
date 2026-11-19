// rev 2026-11-19

using Ans.Net10.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ans.Net10.Web.Middlewares
{

    /// <summary>
    /// Инфраструктурный компонент конвейера (Middleware), обеспечивающий сопряжение данных 
    /// проиндексированной карты сайта с триадой профилей оркеструющего контекста <see cref="CurrentContext"/>.
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
        /// карты сайта за O(1) и наполняет триаду профилей текущего контекста.
        /// </summary>
        /// <param name="context">Контекст текущего обрабатываемого HTTP-запроса.</param>
        /// <param name="current">Оркестровый контекст текущего запроса (Scoped-сервис).</param>
        /// <returns>Задача асинхронного выполнения операции конвейера.</returns>
        /// <exception cref="ArgumentNullException">Вызывается, если <paramref name="context"/> или <paramref name="current"/> равен <see langword="null"/>.</exception>
        public async Task InvokeAsync(
            HttpContext context,
            CurrentContext current)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(current);

            current.Site.Title = current.Options.AppTitle
                ?? SuppApp.CallingAssembly?.Name
                ?? string.Empty;

            var requestPath1 = current.Request.RelativeUrl;
            var pageMapItem1 = current.Sitemap.GetPage(requestPath1);

            if (pageMapItem1 is not null)
            {
                current.Node.MapItem = pageMapItem1.Node;
                current.Page.MapItem = pageMapItem1;
                _log.SitemapPageMatched(_logger, requestPath1);
            }
            else
            {
                var nodeName1 = _extractNodeName(requestPath1);
                var nodeMapItem1 = current.Sitemap.GetNode(nodeName1);

                if (nodeMapItem1 is not null)
                {
                    current.Node.MapItem = nodeMapItem1;
                    _log.SitemapNodeFallbackMatched(_logger, requestPath1, nodeName1);
                }
                else
                {
                    _log.SitemapContextNotFound(_logger, requestPath1);
                }
            }

            await _next(context).ConfigureAwait(false);
        }


        /* privates */


        private static string _extractNodeName(
            string requestPath)
        {
            ReadOnlySpan<char> span1 = requestPath.AsSpan().Trim('/');
            if (span1.IsEmpty)
                return SuppNodes.INDEX_NODE_NAME;

            int slashIndex1 = span1.IndexOf('/');
            return slashIndex1 < 0
                ? span1.ToString()
                : span1[..slashIndex1].ToString();
        }


        /* logs */


        private static partial class _log
        {
            [LoggerMessage(
                Level = LogLevel.Debug,
                Message = "[AnsSitemapProfilerMiddleware] Триада профилей метаданных заполнена по странице карты сайта: '{combinedKey}'.")]
            public static partial void SitemapPageMatched(
                ILogger logger, string combinedKey);

            [LoggerMessage(
                Level = LogLevel.Debug,
                Message = "[AnsSitemapProfilerMiddleware] Страница '{combinedKey}' не найдена в карте; применен профиль родительского узла '{nodeName}'.")]
            public static partial void SitemapNodeFallbackMatched(
                ILogger logger, string combinedKey, string nodeName);

            [LoggerMessage(
                Level = LogLevel.Debug,
                Message = "[AnsSitemapProfilerMiddleware] Запрос по ключу '{combinedKey}' отсутствует в карте сайта. Метаданные триады не заполнены.")]
            public static partial void SitemapContextNotFound(
                ILogger logger, string combinedKey);
        }

    }

}
