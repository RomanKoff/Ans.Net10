using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ans.Net10.Web.Middlewares
{

	/// <summary>
	/// Инфраструктурный компонент конвейера (Middleware), обеспечивающий автоматический 
	/// перманентный редирект (HTTP 301) для URL-адресов, заканчивающихся на закрывающий слэш, 
	/// исключая дублирование контента для поисковых систем (SEO).
	/// </summary>
	public sealed partial class AnsTrailingSlashMiddleware
	{

		private readonly RequestDelegate _next;
		private readonly ILogger<AnsTrailingSlashMiddleware> _logger;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="AnsTrailingSlashMiddleware"/>.
		/// </summary>
		/// <param name="next">Делегат, представляющий следующую функцию обработки в конвейере HTTP.</param>
		/// <param name="logger">Служба ведения системных логов.</param>
		public AnsTrailingSlashMiddleware(
			RequestDelegate next,
			ILogger<AnsTrailingSlashMiddleware> logger)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}


		/* methods */


		/// <summary>
		/// Перехватывает контекст HTTP-запроса и выполняет валидацию структуры URL.
		/// </summary>
		/// <param name="context">Контекст текущего обрабатываемого HTTP-запроса.</param>
		/// <returns>Поток-задача асинхронного выполнения операции конвейера.</returns>
		public async Task InvokeAsync(
			HttpContext context)
		{
			var path1 = context.Request.Path;
			if (path1.HasValue && path1.Value.Length > 1 && path1.Value[^1] == '/')
			{
				var pathSpan1 = path1.Value.AsSpan();
				var cleanPath1 = pathSpan1[..^1];
				var query1 = context.Request.QueryString;
				string targetUrl1 = query1.HasValue
					? $"{cleanPath1}{query1.Value}"
					: cleanPath1.ToString();
				_log.RedirectTrailingSlash(_logger, path1.Value, targetUrl1);
				context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
				context.Response.Headers.Location = targetUrl1;
				return;
			}
			await _next(context);
		}


		/* privates */


		private static partial class _log
		{
			[LoggerMessage(
				EventId = 10,
				Level = LogLevel.Information,
				Message = "[Ans.Net10.Web] Обнаружен дублирующий закрывающий слэш. Выполняется перманентный редирект (HTTP 301): {OldPath} -> {NewPath}")]
			public static partial void RedirectTrailingSlash(ILogger logger, string oldPath, string newPath);
		}

	}

}
