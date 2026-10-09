// rev 2026-10-02

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace Ans.Net10.Web.TagHelpers
{

	/*
	 * <a href-media="https://t.me/new_guap"></a>
	 * <a href-email="user@host.ru"></a>
	 * <a href-tel="+7-123-123-12-12"></a>
	 * <a href-site-res="data/nav.json"></a>
	 * <a href-node-res="docs/otchet.pdf"></a>
	 * <a href-page-res="img1.jpg"></a>
	 * <a href-site="sveden/struct"></a>
	 * <a href-node="docs/forms"></a>
	 * <a href-page="list"></a>
	 */



	/// <summary>
	/// Асинхронный Tag Helper для элемента <c>&lt;a&gt;</c>, расширяющий стандартный <see cref="AnchorTagHelper"/>
	/// возможностями автоматического разбора email, телефонов, медиа-объектов и канонических ссылок Ans Core.
	/// </summary>
	/// <remarks>
	/// Инициализирует новый экземпляр класса <see cref="add_ATagHelper"/> с внедрением системного генератора и контекста.
	/// </remarks>
	/// <param name="generator">Системный генератор HTML-компонентов ASP.NET Core.</param>
	/// <param name="current">Текущий оркестровый контекст выполнения HTTP-запроса.</param>
	[HtmlTargetElement("a", Attributes = _ATTR_HREF_EMAIL)]
	[HtmlTargetElement("a", Attributes = _ATTR_HREF_MEDIA)]
	[HtmlTargetElement("a", Attributes = _ATTR_HREF_NODE)]
	[HtmlTargetElement("a", Attributes = _ATTR_HREF_NODE_RES)]
	[HtmlTargetElement("a", Attributes = _ATTR_HREF_PAGE)]
	[HtmlTargetElement("a", Attributes = _ATTR_HREF_PAGE_RES)]
	[HtmlTargetElement("a", Attributes = _ATTR_HREF_SITE)]
	[HtmlTargetElement("a", Attributes = _ATTR_HREF_SITE_RES)]
	[HtmlTargetElement("a", Attributes = _ATTR_HREF_TEL)]
	[HtmlTargetElement("a", Attributes = _ATTR_MEDIA_AUTO_TITLE)]
	[HtmlTargetElement("a", Attributes = _ATTR_MEDIA_VARIANT)]
	[HtmlTargetElement("a", Attributes = _ATTR_TEL_CODE)]
	public partial class add_ATagHelper(
		IHtmlGenerator generator,
		CurrentContext current)
		: AnchorTagHelper(generator)
	{

		private const string _ATTR_HREF_EMAIL = "href-email";
		private const string _ATTR_HREF_MEDIA = "href-media";
		private const string _ATTR_HREF_NODE = "href-node";
		private const string _ATTR_HREF_NODE_RES = "href-node-res";
		private const string _ATTR_HREF_PAGE = "href-page";
		private const string _ATTR_HREF_PAGE_RES = "href-page-res";
		private const string _ATTR_HREF_SITE = "href-site";
		private const string _ATTR_HREF_SITE_RES = "href-site-res";
		private const string _ATTR_HREF_TEL = "href-tel";
		private const string _ATTR_MEDIA_AUTO_TITLE = "media-auto-title";
		private const string _ATTR_MEDIA_VARIANT = "media-variant";
		private const string _ATTR_TEL_CODE = "tel-code";

		private readonly CurrentContext _current = current;
		private readonly LibWebOptions _options = current.Options;


		/* attributes */


		/// <summary>
		/// Получает или задает URL-адрес медиа-ресурса.
		/// </summary>
		[HtmlAttributeName(_ATTR_HREF_MEDIA)]
		public string? HrefMediaData { get; set; }


		/// <summary>
		/// Определяет, нужно ли генерировать авто-заголовок для медиа-ресурса. По умолчанию: <see langword="true"/>.
		/// </summary>
		[HtmlAttributeName(_ATTR_MEDIA_AUTO_TITLE)]
		public bool MediaAutoTitleData { get; set; } = true;


		/// <summary>
		/// Получает или задает числовой идентификатор варианта медиа-файла.
		/// </summary>
		[HtmlAttributeName(_ATTR_MEDIA_VARIANT)]
		public int MediaVariantData { get; set; } = 0;


		/// <summary>
		/// Получает или задает адрес электронной почты.
		/// </summary>
		[HtmlAttributeName(_ATTR_HREF_EMAIL)]
		public string? HrefEmailData { get; set; }


		/// <summary>
		/// Получает или задает телефонный номер в сыром или строгом формате.
		/// </summary>
		[HtmlAttributeName(_ATTR_HREF_TEL)]
		public string? HrefTelData { get; set; }


		/// <summary>
		/// Получает или задает код телефонного региона по умолчанию.
		/// </summary>
		[HtmlAttributeName(_ATTR_TEL_CODE)]
		public string TelCodeData
		{
			get => field ?? _options.DefaultRegionPhoneCode;
			set;
		}


		/// <summary>
		/// Получает или задает относительный путь к ресурсу уровня сайта.
		/// </summary>
		[HtmlAttributeName(_ATTR_HREF_SITE_RES)]
		public string? HrefSiteResData { get; set; }


		/// <summary>
		/// Получает или задает относительный путь к ресурсу уровня информационного узла.
		/// </summary>
		[HtmlAttributeName(_ATTR_HREF_NODE_RES)]
		public string? HrefNodeResData { get; set; }


		/// <summary>
		/// Получает или задает относительный путь к ресурсу уровня страницы.
		/// </summary>
		[HtmlAttributeName(_ATTR_HREF_PAGE_RES)]
		public string? HrefPageResData { get; set; }


		/// <summary>
		/// Получает или задает внутренний маршрут на раздел уровня сайта.
		/// </summary>
		[HtmlAttributeName(_ATTR_HREF_SITE)]
		public string? HrefSiteData { get; set; }


		/// <summary>
		/// Получает или задает внутренний маршрут на раздел уровня информационного узла.
		/// </summary>
		[HtmlAttributeName(_ATTR_HREF_NODE)]
		public string? HrefNodeData { get; set; }


		/// <summary>
		/// Получает или задает внутренний маршрут на раздел уровня страницы.
		/// </summary>
		[HtmlAttributeName(_ATTR_HREF_PAGE)]
		public string? HrefPageData { get; set; }


		/// <summary>
		/// Асинхронно обрабатывает логику тега ссылки, вычисляет целевой контекст 
		/// и формирует валидные атрибуты href, target и семантические CSS-классы.
		/// </summary>
		/// <param name="context">Контекст выполнения, содержащий информацию о текущем HTML-теге.</param>
		/// <param name="output">Выходной контекст, используемый для формирования результирующего HTML-кода.</param>
		/// <returns>Задача, представляющая асинхронную операцию выполнения генерации тега ссылки.</returns>
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
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			if (!string.IsNullOrEmpty(HrefEmailData))
				await _makeEmailAsync(output, HrefEmailData);
			if (!string.IsNullOrEmpty(HrefTelData))
				await _makeTelAsync(output, HrefTelData, TelCodeData);
			else if (!string.IsNullOrEmpty(HrefSiteResData))
				await _makeHrefAsync(output, _current.GetResUrl($"site:{HrefSiteResData}"));
			else if (!string.IsNullOrEmpty(HrefNodeResData))
				await _makeHrefAsync(output, _current.GetResUrl($"node:{HrefNodeResData}"));
			else if (!string.IsNullOrEmpty(HrefPageResData))
				await _makeHrefAsync(output, _current.GetResUrl($"page:{HrefPageResData}"));
			else if (!string.IsNullOrEmpty(HrefSiteData))
				await _makeHrefAsync(output, _current.GetUrl($"site:{HrefSiteData}"));
			else if (!string.IsNullOrEmpty(HrefNodeData))
				await _makeHrefAsync(output, _current.GetUrl($"node:{HrefNodeData}"));
			else if (!string.IsNullOrEmpty(HrefPageData))
				await _makeHrefAsync(output, _current.GetUrl($"page:{HrefPageData}"));
		}


		/* privates */


		private static async Task _makeEmailAsync(
			TagHelperOutput output,
			string email)
		{
			var parts = email.Split('@');
			if (parts.Length == 2)
			{
				var email1 = $"{parts[0]}@{parts[1]}";
				output.Attributes.SetAttribute("href", new HtmlString($"mailto:{email1}"));
				output.Attributes.Add("itemprop", "email");
				output.AddClass("link-email", HtmlEncoder.Default);
				output.AddClass("text-nowrap", HtmlEncoder.Default);
				var childContent1 = await output.GetChildContentAsync();
				var bodyText1 = childContent1.GetContent();
				output.Content.SetHtmlContent(
					string.IsNullOrEmpty(bodyText1)
						? email1 : bodyText1);
			}
			else
			{
				output.TagName = "span";
				output.Content.SetHtmlContent($"<em>{{ERROR EMAIL FORMAT}}</em>");
			}
		}


		private static async Task _makeTelAsync(
			TagHelperOutput output,
			string tel,
			string code)
		{
			var (text1, href1) = SuppValues.ParsePhoneNumber(tel, code);
			if (string.IsNullOrEmpty(text1))
			{
				output.TagName = "span";
				output.Content.SetHtmlContent($"<em>{{ERROR TELEPHONE NUMBER}}</em>");
				return;
			}
			if (string.IsNullOrEmpty(href1))
				output.TagName = "span";
			else
			{
				output.Attributes.SetAttribute("href", new HtmlString($"tel:{href1}"));
				output.Attributes.Add("itemprop", "telephone");
				output.AddClass("link-telephone", HtmlEncoder.Default);
			}
			output.AddClass("text-nowrap", HtmlEncoder.Default);
			var childContent1 = await output.GetChildContentAsync();
			var bodyText1 = childContent1.GetContent();
			output.Content.SetHtmlContent($"{text1}{bodyText1.Make(" {0}")}");
		}


		private static async Task _makeHrefAsync(
			TagHelperOutput output,
			string url1)
		{
			output.Attributes.SetAttribute("href", new HtmlString(url1));
			var childContent1 = await output.GetChildContentAsync();
			output.Content.SetHtmlContent(childContent1.GetContent());
		}

	}

}
