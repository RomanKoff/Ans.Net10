// rev 2026-09-30

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Базовый класс для моделей страниц Razor (<see cref="PageModel"/>), 
	/// обеспечивающий автоматическое внедрение единого контекста приложения.
	/// </summary>
	/// <param name="current">Текущий оркестровый контекст обработки запроса.</param>
	public abstract class _AnsPageModel_Base(
		CurrentContext current)
		: PageModel
	{

		/// <summary>
		/// Получает единый оркестровый контекст текущего HTTP-запроса.
		/// </summary>
		public CurrentContext Current { get; } = current;

		/// <summary>
		/// Получает строго типизированные параметры конфигурации веб-библиотеки.
		/// </summary>
		public LibWebOptions Options { get; } = current.Options;

	}

}
