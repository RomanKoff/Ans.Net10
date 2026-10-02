// rev 2026-10-02

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ans.Net10.Web.TagHelpers
{

	/// <summary>
	/// Асинхронный Tag Helper для вывода визуального блока отладки, отображающего
	/// текущую активную адаптивную контрольную точку (Breakpoint) экранной сетки Bootstrap.
	/// </summary>
	/// <remarks>
	/// Полезен на этапе разработки и тестирования верстки для контроля поведения
	/// элементов интерфейса на экранах различных разрешений (от XS до XXL).
	/// </remarks>
	[HtmlTargetElement("info-display")]
	public class InfoDisplayTagHelper
		: TagHelper
	{

		/// <summary>
		/// Асинхронно обрабатывает Tag Helper, подавляя генерацию оборачивающего тега
		/// и внедряя в выходной поток HTML-разметку индикаторов адаптивности.
		/// </summary>
		/// <param name="context">Контекст выполнения, содержащий информацию о текущем HTML-теге.</param>
		/// <param name="output">Выходной контекст, используемый для формирования результирующего HTML-кода.</param>
		/// <returns>Задача, представляющая асинхронную операцию выполнения генерации тега.</returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="context"/> или <paramref name="output"/> равен <see langword="null"/>.
		/// </exception>
		/// <remarks>
		/// Метод устанавливает свойство <see cref="TagHelperOutput.TagName"/> в <see langword="null"/>,
		/// благодаря чему сам элемент не рендерится в итоговом HTML, а на его место в режиме 
		/// <see cref="TagMode.StartTagAndEndTag"/> выводится блок диагностических классов <c>d-block</c> и <c>d-none</c>.
		/// </remarks>
		public override Task ProcessAsync(
			TagHelperContext context,
			TagHelperOutput output)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(output);
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = null;
			output.Content.AppendHtml(@"
<div class='debug-display small'>
	<code class='d-block d-sm-none'>XS &lt;576 w100%</code>
	<code class='d-none d-sm-block d-md-none'>SM 576–767 w540</code>
	<code class='d-none d-md-block d-lg-none'>MD 768–991 w720</code>
	<code class='d-none d-lg-block d-xl-none'>LG 992–1199 w960</code>
	<code class='d-none d-xl-block d-xxl-none'>XL 1200–1399 w1140</code>
	<code class='d-none d-xxl-block'>XXL ≥1400 w1320</code>
</div>");
			return Task.CompletedTask;
		}

	}

}
