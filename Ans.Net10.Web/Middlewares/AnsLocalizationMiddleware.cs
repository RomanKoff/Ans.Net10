// rev 2026-10-02

using Ans.Net10.Common;
using Microsoft.AspNetCore.Http;

namespace Ans.Net10.Web.Middlewares
{

	/// <summary>
	/// Компонент конвейера обработки HTTP-запроса (Middleware), обеспечивающий ленивое каскадное извлечение 
	/// региональных настроек (культуры) из параметров строки запроса, файлов cookie или конфигурации по умолчанию, 
	/// и её последующую синхронизацию с текущим потоком выполнения.
	/// </summary>
	/// <param name="next">Делегат, представляющий следующую функцию обработки в конвейере контента.</param>
	/// <param name="options">Строго типизированные параметры глобальной конфигурации веб-библиотеки.</param>
	public class AnsLocalizationMiddleware(
		RequestDelegate next,
		LibWebOptions options)
	{

		/* consts */


		/// <summary>
		/// Ключ параметра в строке запроса (Query String) для явного переключения языка интерфейса (например, "?lang=ru").
		/// </summary>
		public const string CULTURE_QUERY_KEY = "lang";


		/// <summary>
		/// Имя файла cookie платформы ASP.NET Core, используемого для сохранения долгосрочного выбора языка пользователем.
		/// </summary>
		public const string CULTURE_COOKIE_KEY = ".AspNetCore.Culture";


		/* methods */


		/// <summary>
		/// Перехватывает контекст HTTP-запроса, вычисляет целевую культуру по приоритетам и фиксирует её в рантайме.
		/// </summary>
		/// <param name="context">Контекст текущего обрабатываемого HTTP-запроса.</param>
		/// <returns>Поток-задача асинхронного выполнения операции конвейера.</returns>
		public async Task InvokeAsync(
			HttpContext context)
		{
			string? targetCulture1 = context.Request.Query[CULTURE_QUERY_KEY].FirstOrDefault();
			if (string.IsNullOrWhiteSpace(targetCulture1))
				targetCulture1 = context.Request.Cookies[CULTURE_COOKIE_KEY];
			if (string.IsNullOrWhiteSpace(targetCulture1))
				targetCulture1 = options.Culture;
			if (!string.IsNullOrWhiteSpace(targetCulture1))
				try
				{
					SuppCulture.SetCulture(targetCulture1);
				}
				catch
				{
				}
			await next(context);
		}

	}

}
