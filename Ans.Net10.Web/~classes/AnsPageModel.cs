// rev 2026-09-30

namespace Ans.Net10.Web
{

	/// <summary>
	/// Стандартная базовая модель страницы Razor, готовая к использованию в приложении 
	/// и предоставляющая доступ к контексту без необходимости объявления кастомных конструкторов.
	/// </summary>
	/// <param name="current">Текущий оркестровый контекст обработки запроса.</param>
	public class AnsPageModel(
		CurrentContext current)
		: _AnsPageModel_Base(current)
	{

		/// <summary>
		/// Виртуальный метод обработки стандартного HTTP GET-запроса к странице.
		/// </summary>
		/// <remarks>
		/// Может быть переопределен в производных классах для выполнения специфичной 
		/// логики инициализации страницы или обработки входящих параметров.
		/// </remarks>
		public virtual void OnGet()
		{
			// ПРИМЕЧАНИЕ: Будущий функционал анализа маршрутов страниц:
			// var path1 = Current.Request.RazorPage;
			// _ = Current.Request.ParseRequest(path1);
		}

	}

}
