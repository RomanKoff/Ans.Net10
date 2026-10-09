using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Перечисление семантических типов профилей страниц.
	/// </summary>
	public enum PageProfileType
	{
		/// <summary>
		/// Стандартная страница контента (может быть как изолированной, так и иметь дочерние страницы).
		/// </summary>
		Normal,

		/// <summary>
		/// Внутренняя ссылка. Перенаправляет навигацию на другой узел или страницу этого же сайта.
		/// </summary>
		InternalLink,

		/// <summary>
		/// Внешняя ссылка. Ведет на сторонний интернет-ресурс.
		/// </summary>
		ExternalLink
	}



	public sealed class PageProfile
		: _Profile_Proto
	{

		/* ctor */


		public PageProfile()
		{
			Type = PageProfileType.Normal;
			ChildPages = new List<PageProfile>();
		}


		/* properties */


		public string? Name { get; set; } = null;
		public string? Path { get; set; } = null;


		/// <summary>
		/// Получает или задает семантический тип страницы, определяющий характер перехода в интерфейсе.
		/// </summary>
		public PageProfileType Type { get; set; }


		/// <summary>
		/// Получает или задает вышестоящую (родительскую) страницу в иерархическом дереве контента узла.
		/// </summary>
		public PageProfile? ParentPage { get; set; }


		/// <summary>
		/// Получает или задает коллекцию прямых подчиненных (дочерних) контентных страниц.
		/// </summary>
		public ICollection<PageProfile> ChildPages { get; set; }

	}

}
