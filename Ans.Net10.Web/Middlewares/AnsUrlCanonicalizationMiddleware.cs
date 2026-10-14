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
			bool isChanged1 = false;

			// 1. Правило: Удаление закрывающего слэша (за исключением корня сайта "/")
			if (pathSpan1.Length > 1 && pathSpan1[^1] == '/')
			{
				pathSpan1 = pathSpan1[..^1];
				isChanged1 = true;
			}

			// 2. Правило: Удаление скрытого технического суффикса "/start"
			// Проверяем окончание строки на "/start" с игнорированием регистра символов
			if (pathSpan1.EndsWith("/start", StringComparison.OrdinalIgnoreCase))
			{
				pathSpan1 = pathSpan1[..^6];
				isChanged1 = true;

				// Если после удаления /start остался только пустой корень, принудительно делаем его "/"
				if (pathSpan1.IsEmpty)
				{
					pathSpan1 = "/".AsSpan();
				}
			}

			// 3. Правило: Удаление скрытого технического префикса "/_main"
			// Проверяем, начинается ли путь с "/_main" (например, "/_main/contacts" или просто "/_main")
			if (pathSpan1.StartsWith("/_main", StringComparison.OrdinalIgnoreCase))
			{
				if (pathSpan1.Length == 6)
				{
					pathSpan1 = "/".AsSpan();
					isChanged1 = true;
				}
				else if (pathSpan1[6] == '/')
				{
					pathSpan1 = pathSpan1[6..];
					isChanged1 = true;
				}
			}

			// Если сработало хотя бы одно из правил канонизации — выполняем честный HTTP 301 Редирект
			if (isChanged1)
			{
				var query1 = context.Request.QueryString;
				string targetUrl1 = query1.HasValue
					? $"{pathSpan1}{query1.Value}"
					: pathSpan1.ToString();
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
