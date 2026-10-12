using System.Collections.Frozen;

namespace Ans.Net10.Web.Sitemap
{

	/// <summary>
	/// Обеспечивает сборку, индексацию и быстрый доступ к карте узлов и страниц сайта.
	/// </summary>
	public sealed class SitemapBuilder
	{

		private readonly List<NodeItem> _topNodes = [];

		private FrozenDictionary<string, NodeItem> _nodesIndex = FrozenDictionary<string, NodeItem>.Empty;
		private FrozenDictionary<string, PageItem> _pagesIndex = FrozenDictionary<string, PageItem>.Empty;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="SitemapBuilder"/>.
		/// </summary>
		public SitemapBuilder()
		{
			TopNodes = _topNodes.AsReadOnly();
		}


		/* readonly properties */


		/// <summary>
		/// Получает коллекцию корневых (наивысших) узлов карты сайта.
		/// </summary>
		public IReadOnlyCollection<NodeItem> TopNodes { get; }


		/* methods */


		/// <summary>
		/// Возвращает информационный узел по его уникальному имени.
		/// </summary>
		/// <param name="nodeName">Уникальное имя-идентификатор узла.</param>
		/// <returns>Найденный узел <see cref="NodeItem"/> или <see langword="null"/>, если узел не найден.</returns>
		public NodeItem? GetNode(
			string nodeName)
		{
			if (string.IsNullOrEmpty(nodeName))
				return null;
			_nodesIndex.TryGetValue(nodeName, out var node1);
			return node1;
		}


		/// <summary>
		/// Возвращает контентную страницу по ее комбинированному пути (имя_узла/путь_страницы).
		/// </summary>
		/// <param name="combinedPath">Полный ЧПУ-маршрут в формате "nodeName/path".</param>
		/// <returns>Найденная страница <see cref="PageItem"/> или <see langword="null"/>, если страница не найдена.</returns>
		public PageItem? GetPage(
			string combinedPath)
		{
			if (string.IsNullOrEmpty(combinedPath))
				return null;
			_pagesIndex.TryGetValue(combinedPath, out var page1);
			return page1;
		}


		/// <summary>
		/// Возвращает контентную страницу конкретного узла по ее локальному пути.
		/// </summary>
		/// <param name="node">Информационный узел, в рамках которого производится поиск.</param>
		/// <param name="pagePath">Локальный путь страницы (без имени узла).</param>
		/// <returns>Найденная страница <see cref="PageItem"/> или <see langword="null"/>, если страница не найдена.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="node"/> равен <see langword="null"/>.</exception>
		public PageItem? GetPageFromNode(
			NodeItem node,
			string pagePath)
		{
			ArgumentNullException.ThrowIfNull(node);
			if (string.IsNullOrEmpty(pagePath))
				return null;
			string key1 = $"{node.Name}/{pagePath}";
			_pagesIndex.TryGetValue(key1, out var page1);
			return page1;
		}


		internal void AddTopNode(
			NodeItem node)
		{
			if (node != null)
				_topNodes.Add(node);
		}


		/// <summary>
		/// Фиксирует структуру карты сайта, рекурсивно собирает данные и строит высокопроизводительные Frozen-индексы.
		/// </summary>
		internal void BuildIndexes()
		{
			var nodesDict1 = new Dictionary<string, NodeItem>(StringComparer.OrdinalIgnoreCase);
			var pagesDict1 = new Dictionary<string, PageItem>(StringComparer.OrdinalIgnoreCase);
			foreach (var node1 in _topNodes)
				_indexNodeRecursive(node1, nodesDict1, pagesDict1);
			_nodesIndex = nodesDict1.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
			_pagesIndex = pagesDict1.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
		}


		/* privates */


		private static void _indexNodeRecursive(
			NodeItem node,
			Dictionary<string, NodeItem> nodesDict,
			Dictionary<string, PageItem> pagesDict)
		{
			if (!string.IsNullOrEmpty(node.Name))
				nodesDict[node.Name] = node;
			foreach (var page1 in node.TopPages)
				_indexPageRecursive(page1, node.Name, pagesDict);
			foreach (var childNode1 in node.ChildNodes)
				_indexNodeRecursive(childNode1, nodesDict, pagesDict);
		}


		private static void _indexPageRecursive(
			PageItem page,
			string? nodeName,
			Dictionary<string, PageItem> pagesDict)
		{
			if (!string.IsNullOrEmpty(nodeName) && !string.IsNullOrEmpty(page.Path))
			{
				string fullKey1 = $"{nodeName}/{page.Path}";
				pagesDict[fullKey1] = page;
			}
			foreach (var childPage1 in page.ChildPages)
				_indexPageRecursive(childPage1, nodeName, pagesDict);
		}

	}

}
