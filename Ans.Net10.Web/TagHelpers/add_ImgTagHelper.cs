// rev 2026-10-02

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ans.Net10.Web.TagHelpers
{

	/// <summary>
	/// Асинхронный Tag Helper для элемента <c>&lt;img&gt;</c>, обеспечивающий автоматическое 
	/// вычисление и подстановку абсолютных или относительных путей к изображениям 
	/// на основе различных уровней изоляции ресурсов (Site, Node, Page).
	/// </summary>
	/// <remarks>
	/// Инициализирует новый экземпляр класса <see cref="add_ImgTagHelper"/> с внедрением контекста.
	/// </remarks>
	/// <param name="current">Текущий оркестровый контекст выполнения HTTP-запроса.</param>
	/// <example>
	/// &lt;img src-site-res="images/logo1.svg"/&gt;
	/// &lt;img src-node-res="persons/head.png"/&gt;
	/// &lt;img src-page-res="img1.jpg"/&gt;
	/// </example>
	[HtmlTargetElement("img", Attributes = _ATTRS)]
	public partial class add_ImgTagHelper(
		CurrentContext current)
		: _AnsTagHelper_Base(current)
	{

		private const string _ATTR_SRC_SITE_RES = "src-site-res";
		private const string _ATTR_SRC_NODE_RES = "src-node-res";
		private const string _ATTR_SRC_PAGE_RES = "src-page-res";
		private const string _ATTRS = $"{_ATTR_SRC_SITE_RES}, {_ATTR_SRC_NODE_RES}, {_ATTR_SRC_PAGE_RES}";


		/* attributes */


		/// <summary>
		/// Получает или задает имя или путь к ресурсу глобального уровня сайта.
		/// </summary>
		[HtmlAttributeName(_ATTR_SRC_SITE_RES)]
		public string? SrcSiteResData { get; set; }


		/// <summary>
		/// Получает или задает имя или путь к ресурсу уровня текущего информационного узла (Node).
		/// </summary>
		[HtmlAttributeName(_ATTR_SRC_NODE_RES)]
		public string? SrcNodeResData { get; set; }


		/// <summary>
		/// Получает или задает имя или путь к ресурсу уровня текущей изолированной страницы (Page).
		/// </summary>
		[HtmlAttributeName(_ATTR_SRC_PAGE_RES)]
		public string? SrcPageResData { get; set; }


		/* methods */


		/// <summary>
		/// Асинхронно обрабатывает тег изображения, вычисляет целевой URL ресурса 
		/// и принудительно переписывает HTML-атрибуты <c>src</c> и режим закрытия тега.
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
			output.TagName = "img";
			output.TagMode = TagMode.SelfClosing;
			if (!string.IsNullOrEmpty(SrcSiteResData))
				_makeSrc(output, Current.GetResUrl($"site:{SrcSiteResData}"));
			else if (!string.IsNullOrEmpty(SrcNodeResData))
				_makeSrc(output, Current.GetResUrl($"node:{SrcNodeResData}"));
			else if (!string.IsNullOrEmpty(SrcPageResData))
				_makeSrc(output, Current.GetResUrl($"page:{SrcPageResData}"));
		}


		/* privates */


		private static void _makeSrc(
			TagHelperOutput output,
			string url1)
		{
			output.Attributes.SetAttribute("src", new HtmlString(url1));
		}

	}

}
