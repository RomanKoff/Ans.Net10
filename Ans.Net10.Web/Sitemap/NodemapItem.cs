using System.Collections.Frozen;

namespace Ans.Net10.Web.Sitemap
{

	/// <summary>
	/// Описывает иммутабельные данные информационного узла в карте сайта.
	/// </summary>
	public sealed class NodemapItem
	{

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NodemapItem"/>.
		/// </summary>
		public NodemapItem(
			string target,
			string face,
			ICollection<PagemapItem> pagesTree,
			FrozenDictionary<string, PagemapItem> pagesIndex)
		{
			Target = target;
			Face = face;
			PagesTree = pagesTree;
			PagesIndex = pagesIndex;
		}

		/// <summary>
		/// Получает целевой идентификатор или адрес узла.
		/// </summary>
		public string Target { get; }

		/// <summary>
		/// Получает сырую строку автотипографики и заголовков.
		/// </summary>
		public string Face { get; }

		/// <summary>
		/// Получает локальное иерархическое дерево страниц, принадлежащих данному узлу.
		/// </summary>
		public ICollection<PagemapItem> PagesTree { get; }

		/// <summary>
		/// Получает плоский индекс для мгновенного поиска страниц узла по их ЧПУ-пути.
		/// </summary>
		public FrozenDictionary<string, PagemapItem> PagesIndex { get; }

	}

}
