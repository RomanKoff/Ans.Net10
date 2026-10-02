// rev 2026-10-02

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
	[HtmlTargetElement("ans-pagination", Attributes = _ATTRS)]
	public class AnsPaginationTagHelper(
		CurrentContext current)
		: _AnsTagHelper_Base(current)
	{

		private const string _ATTR_MODEL = "model";
		private const string _ATTR_URL_TEMPLATE = "url-template";
		private const string _ATTRS = $"{_ATTR_MODEL}, {_ATTR_URL_TEMPLATE}";


		/* attributes */


		/// <summary>
		/// Неизменяемая структура-модель, инкапсулирующая полное математическое состояние пагинатора 
		/// (текущая страница, общее количество страниц, скользящие диапазоны и т.д.).
		/// </summary>
		[HtmlAttributeName(_ATTR_MODEL)]
		public PaginationModel Model { get; set; }


		/// <summary>
		/// Шаблон URL-адреса для генерации гиперссылок переходов. 
		/// Должен содержать маркер составного форматирования <c>"{0}"</c>, 
		/// вместо которого будет подставляться номер целевой страницы (например, "/catalog?page={0}").
		/// </summary>
		[HtmlAttributeName(_ATTR_URL_TEMPLATE)]
		public string UrlTemplate { get; set; } = string.Empty;


		/* methods */


		/// <summary>
		/// Выполняет асинхронную генерацию и рендеринг разметки контейнера навигации и элементов страниц.
		/// </summary>
		/// <param name="context">Контекст выполнения, содержащий информацию о текущем HTML-теге.</param>
		/// <param name="output">Выходной контекст, используемый для формирования результирующего HTML-кода.</param>
		/// <returns>Задача, представляющая асинхронную операцию выполнения генерации тега.</returns>
		public override async Task ProcessAsync(
			TagHelperContext context,
			TagHelperOutput output)
		{
			if (Model.TotalPages <= 1)
			{
				output.SuppressOutput();
				return;
			}
			output.TagName = "nav";
			output.Attributes.SetAttribute("aria-label", "Page navigation");
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

			output.Content.SetHtmlContent(ul1.ToHtml());
			await Task.CompletedTask;
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
			var liClass1 = "page-item" + (isActive ? " active" : "") + (isDisabled ? " disabled" : "");
			li1.Apply(liClass1, null, null);

			var url1 = string.Format(UrlTemplate, page);
			var link1 = new LinkBuilder(url1, text, isDisabled);
			var a1 = link1.GetTag(null);
			a1.ExpandClassAttribute("page-link");

			li1.InnerHtml.AppendHtml(a1.ToString());
			ul.InnerHtml.AppendHtml(li1.ToString());
		}


		private static void _appendEllipsis(
			TagBuilderExt ul)
		{
			var li1 = new TagBuilderExt("li", TagRenderMode.Normal);
			li1.Apply("page-item disabled", null, null);
			li1.InnerHtml.AppendHtml("<span class=\"page-link\">...</span>");
			ul.InnerHtml.AppendHtml(li1.ToString());
		}

	}

}