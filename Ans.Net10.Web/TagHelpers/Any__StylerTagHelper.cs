// rev 2026-10-02

using Ans.Net10.Common;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ans.Net10.Web.TagHelpers
{

	/// <summary>
	/// Асинхронный Tag Helper общего назначения, перехватывающий рендеринг элементов <c>&lt;div&gt;</c>, 
	/// <c>&lt;span&gt;</c> и <c>&lt;img&gt;</c> для интеллектуального слияния стандартных атрибутов стилизации
	/// с прикладными метаданными <see cref="TagStyler"/>.
	/// </summary>
	/// <example>
	/// &lt;div ans-styler=""&gt;&lt;/div&gt;
	/// &lt;span ans-styler=""&gt;&lt;/span&gt;
	/// &lt;img ans-styler=""/&gt;
	/// </example>
	[HtmlTargetElement("div", Attributes = _ATTRS)]
	[HtmlTargetElement("span", Attributes = _ATTRS)]
	[HtmlTargetElement("img", Attributes = _ATTRS)]
	public partial class Any__StylerTagHelper
		: TagHelper
	{

		private const string _ATTR_ANS_STYLER = "ans-styler";
		private const string _ATTR_CLASS = "class";
		private const string _ATTR_STYLE = "style";
		private const string _ATTRS = $"{_ATTR_ANS_STYLER}, {_ATTR_CLASS}, {_ATTR_STYLE}";


		/* attributes */


		/// <summary>
		/// Получает или задает структурированные данные стилизатора тега Ans Core.
		/// </summary>
		[HtmlAttributeName(_ATTR_ANS_STYLER)]
		public TagStyler? StylerData { get; set; }


		/// <summary>
		/// Получает или задает сырую строку CSS-классов, переданную в стандартном HTML-атрибуте <c>class</c>.
		/// </summary>
		[HtmlAttributeName(_ATTR_CLASS)]
		public string? Class { get; set; }


		/// <summary>
		/// Получает или задает сырую строку инлайновых CSS-стилей, переданную в стандартном HTML-атрибуте <c>style</c>.
		/// </summary>
		[HtmlAttributeName(_ATTR_STYLE)]
		public string? Style { get; set; }


		/* methods */


		/// <summary>
		/// Асинхронно обрабатывает разметку тега, выполняя слияние классов, инлайновых стилей
		/// и обеспечивая неблокирующее извлечение и рендеринг вложенного дочернего контента.
		/// </summary>
		/// <param name="context">Контекст выполнения, содержащий информацию о текущем HTML-теге.</param>
		/// <param name="output">Выходной контекст, используемый для формирования результирующего HTML-кода.</param>
		/// <returns>Задача, представляющая асинхронную операцию выполнения генерации тега.</returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="context"/> или <paramref name="output"/> равен <see langword="null"/>.
		/// </exception>
		public override async Task ProcessAsync(
			TagHelperContext context,
			TagHelperOutput output)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(output);
			await base.ProcessAsync(context, output);
			StylerData ??= new TagStyler();
			StylerData.Classes.ApplyOriginal(Class);
			if (StylerData.Classes.Items.Count > 0)
				output.Attributes.SetAttribute("class", StylerData.Classes.ToString());
			StylerData.Styles.ApplyOriginal(Style);
			if (StylerData.Styles.Items.Count > 0)
				output.Attributes.SetAttribute("style", StylerData.Styles.ToString());
			var content1 = await output.GetChildContentAsync();
			output.Content.AppendHtml(content1);
		}

	}

}
