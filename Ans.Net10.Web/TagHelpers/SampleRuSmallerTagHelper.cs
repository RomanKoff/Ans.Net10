// rev 2026-10-02

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ans.Net10.Web.TagHelpers
{

	/// <summary>
	/// Асинхронный Tag Helper для условного HTML-элемента <c>&lt;sample-ru-smaller&gt;</c>,
	/// предназначенный для генерации оттипографленного ультракороткого демонстрационного «рыба-текста» на русском языке.
	/// </summary>
	[HtmlTargetElement("sample-ru-smaller", TagStructure = TagStructure.WithoutEndTag)]
	public class SampleRuSmallerTagHelper
		: TagHelper
	{
		/// <summary>
		/// Асинхронно обрабатывает Tag Helper, подавляя вывод окружающего HTML-тега
		/// и записывая в выходной поток ультракороткую текстовую заглушку.
		/// </summary>
		/// <param name="context">Контекст выполнения, содержащий информацию о текущем HTML-теге.</param>
		/// <param name="output">Выходной контекст, используемый для формирования результирующего HTML-кода.</param>
		/// <returns>Задача, представляющая асинхронную операцию выполнения генерации тега.</returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="context"/> или <paramref name="output"/> равен <see langword="null"/>.
		/// </exception>
		public override Task ProcessAsync(
			TagHelperContext context,
			TagHelperOutput output)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(output);
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = null;
			output.Content.AppendHtml(SuppRender.SampleSmallerRu());
			return Task.CompletedTask;
		}
	}

}
