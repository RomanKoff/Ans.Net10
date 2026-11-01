using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ans.Net10.Web.Middlewares
{

	/// <summary>
	/// Инфраструктурный компонент конвейера (Middleware), обеспечивающий автоматический 
	/// перманентный редирект (HTTP 301) для приведения URL к каноническому виду:
	/// исключает закрывающие слэши, суффиксы /start и скрывает технический префикс /_main.
	/// </summary>
	public sealed partial class AnsUrlCanonicalizationMiddleware
	{

		private readonly RequestDelegate _next;
		private readonly ILogger<AnsUrlCanonicalizationMiddleware> _logger;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="AnsUrlCanonicalizationMiddleware"/>.
		/// </summary>
		/// <param name="next">Делегат, представляющий следующую функцию обработки в конвейере HTTP.</param>
		/// <param name="logger">Служба ведения системных логов.</param>
		public AnsUrlCanonicalizationMiddleware(
			RequestDelegate next,
			ILogger<AnsUrlCanonicalizationMiddleware> logger)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}


		/* methods */


		/// <summary>
		/// Перехватывает контекст HTTP-запроса и выполняет валидацию каноничности структуры URL.
		/// </summary>
		/// <param name="context">Контекст текущего обрабатываемого HTTP-запроса.</param>
		/// <returns>Поток-задача асинхронного выполнения операции конвейера.</returns>
		public async Task InvokeAsync(
			HttpContext context)
		{
			ArgumentNullException.ThrowIfNull(context);
			string originalPath1 = context.Request.Path.Value ?? string.Empty;

			// Мягко пропускаем статику и API, обрабатываем только ЧПУ-запросы
			if (string.IsNullOrEmpty(originalPath1) || originalPath1.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) || Path.HasExtension(originalPath1))
			{
				await _next(context);
				return;
			}

			var pathSpan1 = originalPath1.AsSpan();
			var pathBaseSpan1 = context.Request.PathBase.Value.AsSpan();
			bool isChanged1 = false;

			// 1. Правило: Удаление закрывающего слэша.
			// Запрещаем удаление, только если это истинный корень сайта (PathBase пустой И Path == "/")
			bool isTrueRoot1 = pathBaseSpan1.IsEmpty && pathSpan1.Length == 1 && pathSpan1[0] == '/';

			if (!isTrueRoot1 && pathSpan1.Length > 0 && pathSpan1[^1] == '/')
			{
				pathSpan1 = pathSpan1[..^1];
				isChanged1 = true;
			}

			// 2. Правило: Удаление скрытого технического суффикса "/start"
			if (pathSpan1.EndsWith("/start", StringComparison.OrdinalIgnoreCase))
			{
				pathSpan1 = pathSpan1[..^6];
				isChanged1 = true;
			}

			// Если после удаления /start или /_main путь стал пустым, но это корень сайта, возвращаем "/"
			if (pathSpan1.IsEmpty && pathBaseSpan1.IsEmpty)
			{
				pathSpan1 = "/".AsSpan();
			}

			// 3. Правило: Удаление скрытого технического префикса "/_main"
			if (pathSpan1.StartsWith("/_main", StringComparison.OrdinalIgnoreCase))
			{
				if (pathSpan1.Length == 6)
				{
					pathSpan1 = pathBaseSpan1.IsEmpty ? "/".AsSpan() : "".AsSpan();
					isChanged1 = true;
				}
				else if (pathSpan1[6] == '/')
				{
					pathSpan1 = pathSpan1[6..];
					isChanged1 = true;
				}
			}

			// Проверяем, есть ли закрывающий слэш в PathBase (например, "/app1/")
			bool isPathBaseTrailingSlash1 = pathBaseSpan1.Length > 0 && pathBaseSpan1[^1] == '/';

			if (isChanged1 || isPathBaseTrailingSlash1)
			{
				var cleanPathBase1 = isPathBaseTrailingSlash1 ? pathBaseSpan1[..^1] : pathBaseSpan1;
				var query1 = context.Request.QueryString;

				string targetUrl1 = query1.HasValue
					? $"{cleanPathBase1}{pathSpan1}{query1.Value}"
					: $"{cleanPathBase1}{pathSpan1}";

				// Защита: если редирект ведет на абсолютно тот же URL, что и текущий, прерываем канонизацию
				string currentFullUrl1 = $"{context.Request.PathBase}{context.Request.Path}";
				if (targetUrl1.Equals(currentFullUrl1, StringComparison.OrdinalIgnoreCase))
				{
					await _next(context);
					return;
				}

				_log.RedirectToCanonicalUrl(_logger, originalPath1, targetUrl1);
				context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
				context.Response.Headers.Location = targetUrl1;
				return;
			}
			await _next(context);
		}


		/* logs */


		private static partial class _log
		{
			[LoggerMessage(
				Level = LogLevel.Information,
				Message = "[Ans.Net10.Web] URL-запрос не является каноническим. Выполняется перманентный редирект (HTTP 301): {OldPath} -> {NewPath}")]
			public static partial void RedirectToCanonicalUrl(ILogger logger, string oldPath, string newPath);
		}

	}

}
