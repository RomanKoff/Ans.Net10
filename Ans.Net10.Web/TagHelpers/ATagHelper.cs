// rev 2026-11-19

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace Ans.Net10.Web.TagHelpers
{

    /*

        <a></a>

    */



    /// <summary>
    /// Расширенный асинхронный Tag Helper для элемента <c>&lt;a&gt;</c>, добавляющий декларативные атрибуты
    /// для генерации ссылок на разделы, страницы, ресурсы сайта/узла/страницы и электронную почту.
    /// </summary>
    /// <remarks>
    /// Инициализирует новый экземпляр класса <see cref="Exts_ATagHelper"/>.
    /// </remarks>
    /// <param name="generator">Стандартный генератор HTML-разметки ASP.NET Core MVC.</param>
    /// <param name="current">Текущий оркестровый контекст выполнения HTTP-запроса.</param>
    /// <example>
    /// &lt;a href-email="user@host.ru"&gt;&lt;/a&gt;
    /// &lt;a href-site-res="data/nav.json"&gt;Навигация&lt;/a&gt;
    /// &lt;a href-node-res="docs/otchet.pdf"&gt;Отчет&lt;/a&gt;
    /// &lt;a href-page-res="img1.jpg"&gt;Изображение&lt;/a&gt;
    /// &lt;a href-site="sveden/struct"&gt;Структура&lt;/a&gt;
    /// &lt;a href-node="docs/forms"&gt;Формы&lt;/a&gt;
    /// &lt;a href-page="list"&gt;Список&lt;/a&gt;
    /// </example>
    [HtmlTargetElement("a", Attributes = _ATTR_HREF_EMAIL)]
    [HtmlTargetElement("a", Attributes = _ATTR_HREF_SITE_RES)]
    [HtmlTargetElement("a", Attributes = _ATTR_HREF_NODE_RES)]
    [HtmlTargetElement("a", Attributes = _ATTR_HREF_PAGE_RES)]
    [HtmlTargetElement("a", Attributes = _ATTR_HREF_SITE)]
    [HtmlTargetElement("a", Attributes = _ATTR_HREF_NODE)]
    [HtmlTargetElement("a", Attributes = _ATTR_HREF_PAGE)]
    public partial class Exts_ATagHelper(
        IHtmlGenerator generator,
        CurrentContext current)
        : AnchorTagHelper(generator)
    {

        private const string _ATTR_HREF_EMAIL = "href-email";
        private const string _ATTR_HREF_SITE_RES = "href-site-res";
        private const string _ATTR_HREF_NODE_RES = "href-node-res";
        private const string _ATTR_HREF_PAGE_RES = "href-page-res";
        private const string _ATTR_HREF_SITE = "href-site";
        private const string _ATTR_HREF_NODE = "href-node";
        private const string _ATTR_HREF_PAGE = "href-page";

        private readonly CurrentContext _current = current
            ?? throw new ArgumentNullException(nameof(current));


        /* properties */


        /// <summary>
        /// Адрес электронной почты для генерации ссылки <c>mailto:</c> с микроразметкой <c>itemprop="email"</c>.
        /// </summary>
        [HtmlAttributeName(_ATTR_HREF_EMAIL)]
        public string? HrefEmailData { get; set; }


        /// <summary>
        /// Относительный путь к статическому ресурсу уровня сайта (<c>site:...</c>).
        /// </summary>
        [HtmlAttributeName(_ATTR_HREF_SITE_RES)]
        public string? HrefSiteResData { get; set; }


        /// <summary>
        /// Относительный путь к статическому ресурсу текущего узла (<c>node:...</c>).
        /// </summary>
        [HtmlAttributeName(_ATTR_HREF_NODE_RES)]
        public string? HrefNodeResData { get; set; }


        /// <summary>
        /// Относительный путь к статическому ресурсу текущей страницы (<c>page:...</c>).
        /// </summary>
        [HtmlAttributeName(_ATTR_HREF_PAGE_RES)]
        public string? HrefPageResData { get; set; }


        /// <summary>
        /// Относительный маршрут уровня сайта (<c>site:...</c>).
        /// </summary>
        [HtmlAttributeName(_ATTR_HREF_SITE)]
        public string? HrefSiteData { get; set; }


        /// <summary>
        /// Относительный маршрут в пределах текущего узла (<c>node:...</c>).
        /// </summary>
        [HtmlAttributeName(_ATTR_HREF_NODE)]
        public string? HrefNodeData { get; set; }


        /// <summary>
        /// Относительный маршрут в пределах текущей страницы (<c>page:...</c>).
        /// </summary>
        [HtmlAttributeName(_ATTR_HREF_PAGE)]
        public string? HrefPageData { get; set; }


        /* methods */


        /// <summary>
        /// Асинхронно обрабатывает расширенные атрибуты ссылки, формируя целевой атрибут <c>href</c>,
        /// вспомогательные CSS-классы и внутреннее содержимое тега <c>&lt;a&gt;</c>.
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

            base.Process(context, output);
            output.TagName = "a";
            output.TagMode = TagMode.StartTagAndEndTag;

            if (!string.IsNullOrEmpty(HrefEmailData))
                await _makeEmailAsync(output, HrefEmailData).ConfigureAwait(false);
            else if (!string.IsNullOrEmpty(HrefSiteResData))
                _makeHref(output, _current.GetResUrl($"site:{HrefSiteResData}"));
            else if (!string.IsNullOrEmpty(HrefNodeResData))
                _makeHref(output, _current.GetResUrl($"node:{HrefNodeResData}"));
            else if (!string.IsNullOrEmpty(HrefPageResData))
                _makeHref(output, _current.GetResUrl($"page:{HrefPageResData}"));
            else if (!string.IsNullOrEmpty(HrefSiteData))
                _makeHref(output, _current.GetSmartUrl($"site:{HrefSiteData}"));
            else if (!string.IsNullOrEmpty(HrefNodeData))
                _makeHref(output, _current.GetSmartUrl($"node:{HrefNodeData}"));
            else if (!string.IsNullOrEmpty(HrefPageData))
                _makeHref(output, _current.GetSmartUrl($"page:{HrefPageData}"));
        }


        /* privates */


        private static async Task _makeEmailAsync(
            TagHelperOutput output,
            string email)
        {
            var a1 = email.Trim().Split('@');
            if (a1.Length == 2 && !string.IsNullOrWhiteSpace(a1[0]) && !string.IsNullOrWhiteSpace(a1[1]))
            {
                var email1 = $"{a1[0]}@{a1[1]}";
                output.Attributes.SetAttribute("href", new HtmlString($"mailto:{email1}"));
                output.Attributes.SetAttribute("itemprop", "email");
                output.AddClass("link-email", HtmlEncoder.Default);
                output.AddClass("text-nowrap", HtmlEncoder.Default);

                var childContent1 = await output.GetChildContentAsync().ConfigureAwait(false);
                var inner1 = childContent1.GetContent();
                output.Content.SetHtmlContent(string.IsNullOrWhiteSpace(inner1) ? email1 : inner1);
            }
            else
            {
                _makeError(output, "ERROR EMAIL FORMAT");
            }
        }


        private static void _makeHref(
            TagHelperOutput output,
            string url)
        {
            output.Attributes.SetAttribute("href", new HtmlString(url));
        }


        private static void _makeError(
            TagHelperOutput output,
            string message)
        {
            output.Content.SetHtmlContent($"<em>{{{message}}}</em>");
        }

    }

}
