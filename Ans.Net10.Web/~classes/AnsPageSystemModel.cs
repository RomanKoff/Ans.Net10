// rev 2026-09-30

using Microsoft.AspNetCore.Mvc;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Специализированная базовая модель для служебных и системных страниц Razor, 
	/// требующих обязательной валидации секретного токена безопасности приложения.
	/// </summary>
	/// <param name="current">Текущий оркестровый контекст обработки запроса.</param>
	public class AnsPageSystemModel(
		CurrentContext current)
		: _AnsPageModel_Base(current)
	{

		/// <summary>
		/// Выполняет валидацию переданного токена безопасности в строке запроса 
		/// перед рендерингом системной страницы.
		/// </summary>
		/// <remarks>
		/// Если глобальный параметр <see cref="LibWebOptions.SystemToken"/> не задан в конфигурации, 
		/// метод генерирует исключение. Если токен в URL отсутствует или не совпадает — возвращает статус 404 (Not Found).
		/// </remarks>
		/// <returns>
		/// Объект <see cref="IActionResult"/>: <see cref="Microsoft.AspNetCore.Mvc.RazorPages.PageResult"/> в случае успешной валидации токена, 
		/// либо <see cref="NotFoundResult"/> при ошибке авторизации.
		/// </returns>
		/// <exception cref="Exception">
		/// Вызывается, если в файле настроек <c>appsettings.json</c> не заполнен обязательный параметр <c>SystemToken</c>.
		/// </exception>
		public virtual IActionResult OnGet()
		{
			if (string.IsNullOrEmpty(Options.SystemToken))
				throw Options.GetExceptionParamRequired("SystemToken");
			var token1 = Current.QueryString.GetString("token");
			if (string.Equals(token1, Options.SystemToken, StringComparison.Ordinal))
				return Page();
			return NotFound();
		}

	}

}
