// rev 2026-09-30

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
		public string? RequestId { get; set; }


		/// <summary>
		/// Получает или задает текстовое сообщение об ошибке.
		/// </summary>
		public string? ExceptionMessage { get; set; }


		/// <summary>
		/// Получает или задает оригинальный URL-путь, на котором произошел сбой до перенаправления.
		/// </summary>
		public string? OriginalPath { get; set; }


		/// <summary>
		/// Получает или задает адрес страницы (Referer), с которой пользователь перешел на текущий URL.
		/// </summary>
		public Uri? RefererUri { get; set; }


		/// <summary>
		/// Получает или задает результирующий HTTP статус-код ошибки.
		/// </summary>
		public int HttpCode { get; set; }


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
			=> RefererUri != null;


		/* virtuals */


		/// <summary>
		/// Выполняет низкоуровневое извлечение диагностических данных об ошибке из фич HTTP-контекста 
		/// и параметров конфигурации библиотеки.
		/// </summary>
		/// <remarks>
		/// Метод является виртуальным и может быть расширен в производных классах для логирования 
		/// или специфичной обработки метаданных.
		/// </remarks>
		public virtual void Init()
		{
			var f1 = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
			var f2 = HttpContext.Features.Get<IExceptionHandlerFeature>();
			RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
			RefererUri = Request.GetTypedHeaders().Referer;
			OriginalPath = f1?.OriginalPath;
			Exception = f2?.Error;
			ExceptionMessage = f2?.Error?.Message;
			ShowInfo = Options.Errors?.ShowInfo ?? false;
		}

	}

}
