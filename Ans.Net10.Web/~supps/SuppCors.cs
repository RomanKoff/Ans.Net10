// rev 2026-09-30

namespace Ans.Net10.Web
{

	/// <summary>
	/// Вспомогательный класс для обеспечения настройки и конфигурации политик CORS (Cross-Origin Resource Sharing).
	/// </summary>
	public static class SuppCors
	{

		/* consts */


		/// <summary>
		/// Системное имя CORS-профиля, разрешающего любые входящие междоменные запросы.
		/// </summary>
		/// <value>
		/// Имеет фиксированное строковое значение <c>"ALLOW ALL"</c>.
		/// </value>
		public const string CORS_ALLOW_ALL = "ALLOW ALL";


		/// <summary>
		/// Системное имя CORS-профиля, разрешающего доступ исключительно списку доверенных внутренних 
		/// или корпоративных доменов (Origins), указанных в конфигурации приложения.
		/// </summary>
		/// <value>
		/// Имеет фиксированное строковое значение <c>"ALLOW TRUSTED"</c>.
		/// </value>
		public const string CORS_ALLOW_TRUSTED = "ALLOW TRUSTED";


		/// <summary>
		/// Системное имя CORS-профиля, разрешающего запросы с локальных адресов разработчиков 
		/// (например, <c>localhost</c> или <c>127.0.0.1</c>) для обеспечения отладки фронтенд-приложений.
		/// </summary>
		/// <value>
		/// Имеет фиксированное строковое значение <c>"ALLOW LOCAL"</c>.
		/// </value>
		public const string CORS_ALLOW_LOCAL = "ALLOW LOCAL";


		/// <summary>
		/// Системное имя CORS-профиля, активирующего поддержку передачи авторизационных данных 
		/// (Cookies, заголовки Authorization клиента) между различными доверенными доменами.
		/// </summary>
		/// <value>
		/// Имеет фиксированное строковое значение <c>"ALLOW CREDENTIALS"</c>.
		/// </value>
		public const string CORS_ALLOW_CREDENTIALS = "ALLOW CREDENTIALS";


		/// <summary>
		/// Системное имя CORS-профиля, разрешающего междоменные запросы исключительно 
		/// для безопасных HTTP-методов чтения данных (<c>GET</c>, <c>HEAD</c>, <c>OPTIONS</c>).
		/// </summary>
		/// <value>
		/// Имеет фиксированное строковое значение <c>"ALLOW READ ONLY"</c>.
		/// </value>
		public const string CORS_ALLOW_READ_ONLY = "ALLOW READ ONLY";

	}

}
