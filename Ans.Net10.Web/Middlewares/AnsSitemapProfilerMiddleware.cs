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
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}


		/* methods */


		/// <summary>
		/// Выполняет перехват канонического URL-адреса запроса, извлекает метаданные из индекса 
		/// карты сайта O(1) и наполняет Триаду профилей текущего контекста.
		/// </summary>
		/// <param name="context">Контекст текущего обрабатываемого HTTP-запроса.</param>
		/// <param name="currentContext">Оркестровый контекст текущего запроса (Scoped-сервис).</param>
		/// <returns>Поток-задача асинхронного выполнения операции конвейера.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="context"/> или <paramref name="currentContext"/> равен <see langword="null"/>.</exception>
		public async Task InvokeAsync(
			HttpContext context,
			CurrentContext currentContext)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(currentContext);

			currentContext.Site.HomeUrl = currentContext.Options.SiteHomeUrl
				?? context.Request.PathBase.Value ?? "/";
			currentContext.Site.SetFace(currentContext.Options.SiteFace
				?? SuppApp.CallingAssembly?.Name ?? "Site");

			string cleanPath1 = (context.Request.Path.Value ?? string.Empty).Trim('/');
			var pageMapItem1 = currentContext.Sitemap.Current.GetPage(cleanPath1);

			if (pageMapItem1 is not null)
			{				
				currentContext.Page.Init(pageMapItem1);
				currentContext.Node.Init(pageMapItem1.Node);
				_log.SitemapContextMatched(_logger, cleanPath1);
			}
			else
			{
				_log.SitemapContextNotFound(_logger, cleanPath1);
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
