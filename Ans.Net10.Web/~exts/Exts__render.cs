// rev 2026-10-19

using Microsoft.AspNetCore.Html;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для логического типа, упрощающие условный рендеринг разметки в Razor-представлениях.
	/// </summary>
	public static partial class Exts__render
	{

		/*

		/// <summary>
		/// Выводит заданный контент Razor только в том случае, если логическое условие истинно.
		/// </summary>
		/// <param name="condition">Логическое условие, управляющее выгрузкой шаблона.</param>
		/// <param name="template">Делегат Razor-шаблона, выполняемый при истинности условия.</param>
		/// <returns>Безопасный HTML-контент для рендеринга на странице.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="template"/> равен <see langword="null"/>.</exception>
		public static IHtmlContent Render(
			this bool condition,
			Func<object?, IHtmlContent> template)
		{
			ArgumentNullException.ThrowIfNull(template);
			return condition
				? template(null)
				: HtmlString.Empty;
		}

		/// <summary>
		/// Выводит первый контент Razor, если логическое условие истинно, или второй контент, если условие ложно.
		/// </summary>
		/// <param name="condition">Логическое условие, управляющее ветвлением рендеринга.</param>
		/// <param name="trueTemplate">Делегат Razor-шаблона, выполняемый при значении <see langword="true"/>.</param>
		/// <param name="falseTemplate">Делегат Razor-шаблона, выполняемый при значении <see langword="false"/>.</param>
		/// <returns>Безопасный HTML-контент для рендеринга на странице.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="trueTemplate"/> или <paramref name="falseTemplate"/> равен <see langword="null"/>.</exception>
		public static IHtmlContent Render(
			this bool condition,
			Func<object?, IHtmlContent> trueTemplate,
			Func<object?, IHtmlContent> falseTemplate)
		{
			ArgumentNullException.ThrowIfNull(trueTemplate);
			ArgumentNullException.ThrowIfNull(falseTemplate);
			return condition
				? trueTemplate(null)
				: falseTemplate(null);
		}

		*/

	}

}
