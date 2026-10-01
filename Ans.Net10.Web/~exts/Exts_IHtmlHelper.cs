// rev 2026-09-30

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для <see cref="IHtmlHelper"/>, упрощающие программное извлечение, 
	/// материализацию и форматирование динамического контента Razor-представлений в строки и HTML-сущности.
	/// </summary>
	public static partial class Exts_IHtmlHelper
	{

		/// <summary>
		/// Преобразует изолированный HTML-контент Razor (лямбда-выражение) в стандартную строку.
		/// </summary>
		/// <param name="helper">Текущий экземпляр помощника <see cref="IHtmlHelper"/>.</param>
		/// <param name="html">Делегат, представляющий Razor-разметку или HTML-компонент.</param>
		/// <returns>Строка, содержащая отрендеренный HTML-код.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="helper"/> или <paramref name="html"/> равен <see langword="null"/>.</exception>
		public static string GetStringFromRazor(
			this IHtmlHelper helper,
			Func<dynamic, IHtmlContent> html)
		{
			ArgumentNullException.ThrowIfNull(helper);
			return helper.ViewContext.HttpContext.GetStringFromRazor(html);
		}


		/// <summary>
		/// Материализует массив Razor-делегатов в строки и подставляет их в указанный текстовый шаблон по правилам составного форматирования.
		/// </summary>
		/// <param name="helper">Текущий экземпляр помощника <see cref="IHtmlHelper"/>.</param>
		/// <param name="template">Строка составного формата (шаблон) с маркерами подстановки вида {0}, {1} и т.д.</param>
		/// <param name="args">Массив Razor-делегатов, результаты рендеринга которых подставляются в шаблон.</param>
		/// <returns>Результирующая отформатированная текстовая строка.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="helper"/>, <paramref name="template"/> или <paramref name="args"/> равен <see langword="null"/>.</exception>
		public static string GetStringFromRazor(
			this IHtmlHelper helper,
			string template,
			params Func<dynamic, IHtmlContent>[] args)
		{
			ArgumentNullException.ThrowIfNull(helper);
			ArgumentNullException.ThrowIfNull(template);
			ArgumentNullException.ThrowIfNull(args);
			var context1 = helper.ViewContext.HttpContext;
			var stringArgs1 = args.Select(context1.GetStringFromRazor).ToArray();
			return string.Format(template, stringArgs1);
		}


		/// <summary>
		/// Рендерит HTML-контент Razor и оборачивает его в безопасную структуру <see cref="HtmlString"/>, готовую к выводу без повторного экранирования.
		/// </summary>
		/// <param name="helper">Текущий экземпляр помощника <see cref="IHtmlHelper"/>.</param>
		/// <param name="html">Делегат, представляющий Razor-разметку или HTML-компонент.</param>
		/// <returns>Объект класса <see cref="HtmlString"/> с отрендеренным контентом.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="helper"/> или <paramref name="html"/> равен <see langword="null"/>.</exception>
		public static HtmlString GetHtmlFromRazor(
			this IHtmlHelper helper,
			Func<dynamic, IHtmlContent> html)
		{
			ArgumentNullException.ThrowIfNull(helper);
			return new HtmlString(
				helper.GetStringFromRazor(html));
		}


		/// <summary>
		/// Формирует строку по шаблону на основе отрендеренных Razor-компонентов и возвращает результат в виде безопасного объекта <see cref="HtmlString"/>.
		/// </summary>
		/// <param name="helper">Текущий экземпляр помощника <see cref="IHtmlHelper"/>.</param>
		/// <param name="template">Строка составного формата (шаблон) с маркерами подстановки.</param>
		/// <param name="args">Массив Razor-делегатов для подстановки в шаблон.</param>
		/// <returns>Объект класса <see cref="HtmlString"/>, содержащий безопасный отформатированный HTML-код.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="helper"/>, <paramref name="template"/> или <paramref name="args"/> равен <see langword="null"/>.</exception>
		public static HtmlString GetHtmlFromRazor(
			this IHtmlHelper helper,
			string template,
			params Func<dynamic, IHtmlContent>[] args)
		{
			ArgumentNullException.ThrowIfNull(helper);
			return new HtmlString(
				helper.GetStringFromRazor(template, args));
		}

	}

}
