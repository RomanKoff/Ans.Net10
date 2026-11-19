// rev 2026-11-19

using Ans.Net10.Common;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ans.Net10.Web.TagHelpers
{

    /*
      
        <div ans-styler="" class="" style=""></div>
        <span ans-styler="" class="" style=""></span>
        <img ans-styler="" src="" class="" style="" />

	 */



    /// <summary>
    /// Асинхронный Tag Helper общего назначения, перехватывающий рендеринг элементов <c>&lt;div&gt;</c>, 
    /// <c>&lt;span&gt;</c> и <c>&lt;img&gt;</c> для интеллектуального слияния стандартных атрибутов стилизации
    /// с прикладными метаданными <see cref="TagStyler"/>.
    /// </summary>
    /// <example>
    /// &lt;div ans-styler="@styler"&gt;&lt;/div&gt;
    /// &lt;span ans-styler="@styler" class="extra"&gt;&lt;/span&gt;
    /// &lt;img ans-styler="@styler" src="..." /&gt;
    /// </example>
    [HtmlTargetElement("div", Attributes = _ATTR_ANS_STYLER)]
    [HtmlTargetElement("span", Attributes = _ATTR_ANS_STYLER)]
    [HtmlTargetElement("img", Attributes = _ATTR_ANS_STYLER)]
    public partial class _StylerTagHelper
        : TagHelper
    {

        private const string _ATTR_ANS_STYLER = "ans-styler";
        private const string _ATTR_CLASS = "class";
        private const string _ATTR_STYLE = "style";


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
        /// Асинхронно обрабатывает разметку тега, выполняя слияние классов и инлайновых стилей.
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

            StylerData ??= new TagStyler();
            StylerData.Classes.ApplyOriginal(Class);
            if (StylerData.Classes.Items.Count > 0)
                output.Attributes.SetAttribute(_ATTR_CLASS, StylerData.Classes.ToString());

            StylerData.Styles.ApplyOriginal(Style);
            if (StylerData.Styles.Items.Count > 0)
                output.Attributes.SetAttribute(_ATTR_STYLE, StylerData.Styles.ToString());

            return Task.CompletedTask;
        }

    }

}
