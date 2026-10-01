// rev 2026-010-01

using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для компонентов представления Razor (<see cref="IView"/>, 
	/// <see cref="IRazorPage"/>, <see cref="RazorPage"/>), упрощающие извлечение метаданных и управление рендерингом.
	/// </summary>
	public static partial class Exts_View
	{

		/// <summary>
		/// Возвращает чистое имя файла представления (без пути и расширения) для указанного <see cref="IView"/>.
		/// </summary>
		/// <param name="view">Текущий экземпляр движка представления.</param>
		/// <returns>Строка с именем представления. Если путь пуст или равен <see langword="null"/>, возвращает пустую строку.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="view"/> равен <see langword="null"/>.</exception>
		public static string GetViewName(
			this IView view)
		{
			ArgumentNullException.ThrowIfNull(view);
			if (string.IsNullOrEmpty(view.Path))
				return string.Empty;
			return Path.GetFileNameWithoutExtension(view.Path);
		}


		/// <summary>
		/// Возвращает чистое имя файла страницы Razor (без пути и расширения) для указанной <see cref="IRazorPage"/>.
		/// </summary>
		/// <param name="view">Текущий экземпляр страницы Razor.</param>
		/// <returns>Строка с именем страницы. Если путь пуст или равен <see langword="null"/>, возвращает пустую строку.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="view"/> равен <see langword="null"/>.</exception>
		public static string GetViewName(
			this IRazorPage view)
		{
			ArgumentNullException.ThrowIfNull(view);
			if (string.IsNullOrEmpty(view.Path))
				return string.Empty;
			return Path.GetFileNameWithoutExtension(view.Path);
		}


		/// <summary>
		/// Выполняет безопасную асинхронную попытку рендеринга именованной секции Razor-страницы.
		/// </summary>
		/// <remarks>
		/// Проверяет, определена ли секция в дочернем представлении перед запуском её рендеринга. 
		/// Позволяет избежать генерации системных исключений при отсутствии необязательных блоков разметки.
		/// </remarks>
		/// <param name="page">Текущий рабочий контекст страницы Razor.</param>
		/// <param name="sectionName">Системное уникальное имя проверяемой секции.</param>
		/// <returns>
		/// Задача, результатом выполнения которой является <see langword="true"/>, если секция успешно найдена и отрендерена; 
		/// в противном случае (если секция не определена) — <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="page"/> или <paramref name="sectionName"/> равен <see langword="null"/>.
		/// </exception>
		public static async Task<bool> TrySectionAsync(
			this RazorPage page,
			string sectionName)
		{
			ArgumentNullException.ThrowIfNull(page);
			ArgumentNullException.ThrowIfNull(sectionName);
			if (!page.IsSectionDefined(sectionName))
				return false;
			await page.RenderSectionAsync(sectionName);
			return true;
		}

	}

}
