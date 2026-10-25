// rev 2026-10-23

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ans.Net10.Web.TagHelpers
{

	/*
	 * <site-container>...</site-container>
	 * <node-container>...</node-container>
	 * <page-container>...</page-container>	 
	 */



	/// <summary>
	/// Тег-хелпер для генерации контейнера сайта (<c>&lt;site-container&gt;</c>).
	/// </summary>
	[HtmlTargetElement("site-container")]
	public class SiteContainerTagHelper(
		CurrentContext current)
		: _AnsTagHelper_Base(current)
	{

		/// <summary>
		/// Асинхронно обрабатывает тег-хелпер, настраивая тег контейнера сайта.
		/// </summary>
		/// <param name="context">Контекст выполнения тег-хелпера.</param>
		/// <param name="output">Выходные данные для генерации HTML-тега.</param>
		/// <returns>Задача, представляющая асинхронную операцию обработки.</returns>
		public override Task ProcessAsync(
			TagHelperContext context,
			TagHelperOutput output)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(output);
			UseAutoContent = true;
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "div";
			ClassBefore = Current.Site.ContainerClasses;
			return base.ProcessAsync(context, output);
		}

	}



	/// <summary>
	/// Тег-хелпер для генерации контейнера узла (<c>&lt;node-container&gt;</c>).
	/// </summary>
	[HtmlTargetElement("node-container")]
	public class NodeContainerTagHelper(
		CurrentContext current)
		: _AnsTagHelper_Base(current)
	{

		/// <summary>
		/// Асинхронно обрабатывает тег-хелпер, настраивая тег контейнера узла.
		/// </summary>
		/// <param name="context">Контекст выполнения тег-хелпера.</param>
		/// <param name="output">Выходные данные для генерации HTML-тега.</param>
		/// <returns>Задача, представляющая асинхронную операцию обработки.</returns>
		public override Task ProcessAsync(
			TagHelperContext context,
			TagHelperOutput output)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(output);
			UseAutoContent = true;
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "div";
			ClassBefore = Current.Node.ContainerClasses;
			return base.ProcessAsync(context, output);
		}

	}



	/// <summary>
	/// Тег-хелпер для генерации контейнера страницы (<c>&lt;page-container&gt;</c>).
	/// </summary>
	[HtmlTargetElement("page-container")]
	public class PageContainerTagHelper(
		CurrentContext current)
		: _AnsTagHelper_Base(current)
	{

		/// <summary>
		/// Асинхронно обрабатывает тег-хелпер, настраивая тег контейнера страницы.
		/// </summary>
		/// <param name="context">Контекст выполнения тег-хелпера.</param>
		/// <param name="output">Выходные данные для генерации HTML-тега.</param>
		/// <returns>Задача, представляющая асинхронную операцию обработки.</returns>
		public override Task ProcessAsync(
			TagHelperContext context,
			TagHelperOutput output)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(output);
			UseAutoContent = true;
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "div";
			ClassBefore = Current.Page.ContainerClasses;
			return base.ProcessAsync(context, output);
		}

	}

}
