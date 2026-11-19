// rev 2026-11-19

using Ans.Net10.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ans.Net10.Web.TagHelpers
{

    /// <summary>
    /// Асинхронный Tag Helper для генерации HTML-разметки постраничной навигации (пагинации) 
    /// в стиле CSS-фреймворка Bootstrap 5 на основе объектной модели <see cref="PaginationModel"/>.
    /// </summary>
    /// <param name="current">Текущий оркестровый контекст выполнения HTTP-запроса.</param>
    [HtmlTargetElement("ans-pagination", Attributes = _ATTR_MODEL, TagStructure = TagStructure.WithoutEndTag)]
    public class AnsPaginationTagHelper(
        CurrentContext current)
        : _AnsTagHelper_Base(current)
    {

        private const string _ATTR_MODEL = "model";
        private const string _ATTR_URL_TEMPLATE = "url-template";
        private const string _ATTR_PAGE_PARAM = "page-param";


        /* attributes */


        /// <summary>
        /// Неизменяемая структура-модель, инкапсулирующая полное математическое состояние пагинатора 
        /// (текущая страница, общее количество страниц, скользящие диапазоны и т.д.).
        /// </summary>
        [HtmlAttributeName(_ATTR_MODEL)]
        public PaginationModel Model { get; set; }


        /// <summary>
        /// Опциональный шаблон URL-адреса для генерации гиперссылок переходов. 
        /// Если задан, должен содержать маркер <c>"{0}"</c> для номера страницы (например, <c>"/catalog/page-{0}"</c>).
        /// Если не указан, ссылки строятся автоматически через <see cref="CurrentContext.QueryString"/> с сохранением текущих параметров запроса.
        /// </summary>
        [HtmlAttributeName(_ATTR_URL_TEMPLATE)]
        public string? UrlTemplate { get; set; }


        /// <summary>
        /// Имя параметра строки запроса для номера страницы при автоматической генерации URL (по умолчанию <c>"page"</c>).
        /// </summary>
        [HtmlAttributeName(_ATTR_PAGE_PARAM)]
        public string PageParam { get; set; } = "page";


        /* methods */


        /// <summary>
        /// Выполняет асинхронную генерацию и рендеринг разметки контейнера навигации и элементов страниц.
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

            if (Model.TotalPages <= 1)
            {
                output.SuppressOutput();
                return Task.CompletedTask;
            }

            output.TagName = "nav";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("aria-label", "Page navigation");
            output.AddAttributeIfPresent("class", ClassBefore, Class, ClassAfter);
            output.AddAttributeIfPresent("style", Style);
            output.AddAttributeIfPresent("title", Title);

            var ul1 = new TagBuilderExt("ul", TagRenderMode.Normal);
            ul1.Apply("pagination justify-content-center", null, null);

            // Кнопка "Назад"
            _appendPageItem(ul1, Model.CurrentPage - 1, "«", Model.ActiveFirstPage);

            // Левое многоточие и ссылка на первую страницу
            if (Model.HasItemsBefore)
            {
                _appendPageItem(ul1, 1, "1", false);
                _appendEllipsis(ul1);
            }

            // Цикл по доступным видимым страницам скользящего диапазона
            for (int i1 = Model.StartPage; i1 <= Model.EndPage; i1++)
                _appendPageItem(ul1, i1, i1.ToString(), false, i1 == Model.CurrentPage);

            // Правое многоточие и ссылка на последнюю страницу
            if (Model.HasItemsAfter)
            {
                _appendEllipsis(ul1);
                _appendPageItem(ul1, Model.TotalPages, Model.TotalPages.ToString(), false);
            }

            // Кнопка "Вперед"
            _appendPageItem(ul1, Model.CurrentPage + 1, "»", Model.ActiveLastPage);

            output.Content.SetHtmlContent(ul1);
            return Task.CompletedTask;
        }


        /* privates */


        private void _appendPageItem(
            TagBuilderExt ul,
            int page,
            string text,
            bool isDisabled,
            bool isActive = false)
        {
            var li1 = new TagBuilderExt("li", TagRenderMode.Normal);
            var activeClass1 = isActive ? " active" : string.Empty;
            var disabledClass1 = isDisabled ? " disabled" : string.Empty;
            li1.Apply($"page-item{activeClass1}{disabledClass1}", null, null);

            var url1 = _buildPageUrl(page);
            var link1 = new LinkBuilder(url1, text, isDisabled);
            var a1 = link1.GetTag(null);
            a1.ExpandClassAttribute("page-link");

            li1.InnerHtml.AppendHtml(a1);
            ul.InnerHtml.AppendHtml(li1);
        }


        private string _buildPageUrl(
            int page)
        {
            if (!string.IsNullOrWhiteSpace(UrlTemplate))
                return string.Format(UrlTemplate, page);

            var queryHelper1 = Current.QueryString.GetHelper();
            if (page > 1)
                queryHelper1.AppendString(PageParam, page.ToString());
            else
                queryHelper1.Remove(PageParam);

            return queryHelper1.MakeQueryString(Current.Request.RelativeUrl);
        }


        private static void _appendEllipsis(
            TagBuilderExt ul)
        {
            var li1 = new TagBuilderExt("li", TagRenderMode.Normal);
            li1.Apply("page-item disabled", null, null);
            li1.InnerHtml.AppendHtml("""<span class="page-link">...</span>""");
            ul.InnerHtml.AppendHtml(li1);
        }

    }

}
