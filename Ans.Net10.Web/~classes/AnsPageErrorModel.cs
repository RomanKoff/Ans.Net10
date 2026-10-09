// rev 2026-10-09

using Ans.Net10.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Базовая модель страницы для отображения и анализа системных или HTTP ошибок приложения.
	/// </summary>
	/// <param name="current">Текущий оркестровый контекст обработки запроса.</param>
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	[IgnoreAntiforgeryToken]
	public class AnsPageErrorModel(
		CurrentContext current)
		: _AnsPageModel_Base(current)
	{

		/* properties */


		/// <summary>
		/// Получает или задает пойманное системное исключение.
		/// </summary>
		public Exception? Exception { get; set; }


		/// <summary>
		/// Получает или задает уникальный идентификатор текущего запроса для трассировки логов.
		/// </summary>
		public string RequestId { get; set; } = string.Empty;


		/// <summary>
		/// Получает или задает текстовое сообщение об ошибке.
		/// </summary>
		public string ExceptionMessage { get; set; } = string.Empty;


		/// <summary>
		/// Получает или задает оригинальный URL-путь, на котором произошел сбой до перенаправления.
		/// </summary>
		public string OriginalPath { get; set; } = string.Empty;


		/// <summary>
		/// Получает или задает адрес страницы (Referer), с которой пользователь перешел на текущий URL.
		/// </summary>
		public string RefererUri { get; set; } = string.Empty;


		/// <summary>
		/// Получает или задает результирующий HTTP статус-код ошибки.
		/// </summary>
		public int HttpCode { get; set; } = 500;


		/// <summary>
		/// Получает или задает признак, разрешающий вывод детальной технической информации на экран.
		/// </summary>
		public bool ShowInfo { get; set; }


		/* readonly properties */


		/// <summary>
		/// Возвращает признак наличия идентификатора трассировки запроса.
		/// </summary>
		public bool HasRequestId
			=> !string.IsNullOrEmpty(RequestId);


		/// <summary>
		/// Возвращает признак наличия текстового сообщения об ошибке.
		/// </summary>
		public bool HasExceptionMessage
			=> !string.IsNullOrEmpty(ExceptionMessage);

		/// <summary>
		/// Возвращает признак наличия оригинального пути запроса.
		/// </summary>
		public bool HasOriginalPath
			=> !string.IsNullOrEmpty(OriginalPath);


		/// <summary>
		/// Возвращает признак наличия информации о странице-источнике перехода (Referer).
		/// </summary>
		public bool HasRefererUri
			=> !string.IsNullOrEmpty(RefererUri);


		/* virtuals */


		/// <summary>
		/// Выполняет низкоуровневое извлечение диагностических данных об ошибке из фич HTTP-контекста 
		/// и параметров конфигурации библиотеки.
		/// </summary>
		public virtual void Init()
		{
			ShowInfo = Current.Options.Errors?.ShowInfo ?? false;
			RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
			RefererUri = HttpContext.Request.Headers.Referer.ToString();

			// СЦЕНАРИЙ 1: Фатальные ошибки (Исключения / throw), перехваченные app.UseExceptionHandler()
			var exceptionFeature1 = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
			if (exceptionFeature1 != null)
			{
				HttpCode = 500;
				Exception = exceptionFeature1.Error;
				OriginalPath = exceptionFeature1.Path;
				ExceptionMessage = exceptionFeature1.Error.GetExceptionMessage();
				//if (Current.Logger.IsEnabled(LogLevel.Critical))
				//	Current.Logger.LogCritical(
				//		500,
				//		"Server{HttpCode} {OriginalPath} {RefererUri} {@Exception}",
				//		HttpCode, OriginalPath, RefererUri, Exception);
				return;
			}

			// СЦЕНАРИЙ 2: Статус-коды (404, 403, 400), перехваченные app.UseStatusCodePagesWithReExecute()
			var statusCodeFeature1 = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
			if (statusCodeFeature1 != null)
			{
				HttpCode = HttpContext.Response.StatusCode;
				OriginalPath = statusCodeFeature1.OriginalPath;
				if (HttpContext.Request.Query.TryGetValue("code", out var codeStr1)
					&& int.TryParse(codeStr1, out var parsedCode1))
					HttpCode = parsedCode1;
				ExceptionMessage = HttpCode switch
				{
					400 => Resources.Errors.Text_BadRequest,
					403 => Resources.Errors.Text_AccessIsDenied,
					404 => Resources.Errors.Text_PageNotFound,
					_ => string.Format(Resources.Errors.Template_HttpError, HttpCode)
				};
				if (HttpCode == 404)
				{
					if (Current.Logger.IsEnabled(LogLevel.Warning))
						Current.Logger.LogWarning(
							404,
							"Http{HttpCode} {OriginalPath} {RefererUri} {@Exception}",
							HttpCode, OriginalPath, RefererUri, ExceptionMessage);
				}
				else
				{
					if (Current.Logger.IsEnabled(LogLevel.Error))
						Current.Logger.LogError(
							400,
							"Http{HttpCode} {OriginalPath} {RefererUri} {@Exception}",
							HttpCode, OriginalPath, RefererUri, ExceptionMessage);
				}
			}
		}

	}

}
