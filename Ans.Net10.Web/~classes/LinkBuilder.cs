// rev 2026-10-01

using Ans.Net10.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Построитель гиперссылок (<c>&lt;a&gt;</c>) и заглушек (<c>&lt;span&gt;</c>), инкапсулирующий логику 
	/// управления состояниями активности, блокировки, внешних переходов и применения базовой типографики.
	/// </summary>
	public class LinkBuilder
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="LinkBuilder"/> с автоматическим применением базовой типографики к тексту ссылки.
		/// </summary>
		/// <param name="href">Целевой URL-адрес гиперссылки.</param>
		/// <param name="title">Отображаемый текст (заголовок) ссылки.</param>
		/// <param name="isDisabled">Признак принудительной блокировки ссылки. По умолчанию: <see langword="false"/>.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="title"/> равен <see langword="null"/>.</exception>
		public LinkBuilder(
			string? href,
			string title,
			bool isDisabled = false)
		{
			ArgumentNullException.ThrowIfNull(title);
			Href = href ?? string.Empty;
			InnerHtml = SuppTypograph.GetTypografMin(title) ?? Href;
			IsDisabled = isDisabled;
		}


		/* properties */


		/// <summary>
		/// Уникальный идентификатор HTML-элемента (атрибут <c>id</c>).
		/// </summary>
		public string? Id { get; set; }


		/// <summary>
		/// Список CSS-классов, добавляемых к итоговому HTML-тегу (атрибут <c>class</c>).
		/// </summary>
		public string? CssClass { get; set; }


		/// <summary>
		/// Целевой URL-адрес гиперссылки (атрибут <c>href</c>).
		/// </summary>
		public string Href { get; set; }


		/// <summary>
		/// Материализованное строковое содержимое (HTML/текст) внутри тега ссылки.
		/// </summary>
		public string InnerHtml { get; set; }


		/// <summary>
		/// Определяет, ведет ли ссылка на внешний ресурс. Если <see langword="true"/>, добавляется атрибут <c>target="_blank"</c>.
		/// </summary>
		public bool IsExternal { get; set; }


		/// <summary>
		/// Определяет, является ли ссылка активной (соответствующей текущей странице). Если <see langword="true"/>, добавляется атрибут <c>aria-current="page"</c>.
		/// </summary>
		public bool IsActive { get; set; }


		/// <summary>
		/// Вспомогательный признак активности родительского или смежного элемента навигации.
		/// </summary>
		public bool IsSubActive { get; set; }


		/// <summary>
		/// Определяет, заблокирована ли ссылка. Если <see langword="true"/> или <see cref="Href"/> пуста, тег рендерится как <c>&lt;span&gt;</c>.
		/// </summary>
		public bool IsDisabled { get; set; }


		/* functions */


		/// <summary>
		/// Формирует и возвращает настроенный экземпляр HTML-компонента <see cref="TagBuilderExt"/> на основе текущего состояния свойств.
		/// </summary>
		/// <remarks>
		/// Если ссылка заблокирована (<see cref="IsDisabled"/> или <see cref="Href"/> пуста), генерируется тег <c>&lt;span&gt;</c> с атрибутами доступности 
		/// <c>aria-disabled="true"</c> и <c>tabindex="-1"</c>. В противном случае генерируется стандартный тег <c>&lt;a&gt;</c>.
		/// </remarks>
		/// <param name="innerHtml">Опциональное кастомное HTML-содержимое для переопределения значения <see cref="InnerHtml"/>.</param>
		/// <returns>Полностью инициализированный и готовый к рендерингу объект расширенного тега <see cref="TagBuilderExt"/>.</returns>
		public TagBuilderExt GetTag(
			string? innerHtml = null)
		{
			TagBuilderExt tag1;
			if (IsDisabled || string.IsNullOrEmpty(Href))
			{
				tag1 = new("span", TagRenderMode.Normal);
				tag1.AddCssClass("link-disabled disabled opacity-75");
				tag1.MergeAttribute("tabindex", "-1");
				tag1.MergeAttribute("aria-disabled", "true");
			}
			else
			{
				tag1 = new("a", TagRenderMode.Normal);
				tag1.MergeAttribute("href", SuppValues.Default(Href, "/"));
				if (IsActive)
				{
					tag1.AddCssClass("active");
					tag1.MergeAttribute("aria-current", "page");
				}
				if (IsExternal)
				{
					tag1.AddCssClass("link-external");
					tag1.MergeAttribute("target", "_blank");
				}
			}
			if (!string.IsNullOrEmpty(Id))
				tag1.MergeAttribute("id", Id);
			if (!string.IsNullOrEmpty(CssClass))
				tag1.AddCssClass(CssClass);
			tag1.InnerHtml.AppendHtml(innerHtml ?? InnerHtml);
			return tag1;
		}

	}

}
