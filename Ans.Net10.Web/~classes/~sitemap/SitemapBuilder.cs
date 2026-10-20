// rev 2026-10-20

using System.Collections.Frozen;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Строитель и контейнер графа навигации, обеспечивающий индексацию и быстрый 
	/// регистронезависимый поиск узлов и страниц карты сайта в оперативной памяти.
	/// </summary>
	public sealed class SitemapBuilder
	{

		private readonly List<NodemapItem> _topNodes = [];


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
		/// Получает коллекцию корневых информационных узлов карты сайта.
		/// </summary>
		public IReadOnlyCollection<NodemapItem> TopNodes { get; }


		/// <summary>
		/// Получает индексный словарь информационных узлов карты сайта.
		/// </summary>
		public FrozenDictionary<string, NodemapItem> NodesIndex { get; private set; }
			= FrozenDictionary<string, NodemapItem>.Empty;


		/// <summary>
		/// Получает индексный словарь информационных страниц карты сайта.
		/// </summary>
		public FrozenDictionary<string, PagemapItem> PagesIndex { get; private set; }
			= FrozenDictionary<string, PagemapItem>.Empty;


		/* functions */


		/// <summary>
		/// Возвращает информационный узел из индекса по его системному имени.
		/// </summary>
		/// <param name="nodeName">Системное имя искомого информационного узла.</param>
		/// <returns>Найденный экземпляр <see cref="NodemapItem"/> или <see langword="null"/>, если узел не найден или имя пусто.</returns>
		public NodemapItem? GetNode(
			string nodeName)
		{
			NodesIndex.TryGetValue(nodeName, out var node1);
			return node1;
		}


		/// <summary>
		/// Возвращает контентную страницу из индекса по её полному составному пути.
		/// </summary>
		/// <param name="combinedPath">Составной ЧПУ-путь страницы в формате "ИмяУзла/ПутьСтраницы".</param>
		/// <returns>Найденный экземпляр <see cref="PagemapItem"/> или <see langword="null"/>, если страница не найдена.</returns>
		public PagemapItem? GetPage(
			string combinedPath)
		{
			PagesIndex.TryGetValue(combinedPath, out var page1);
			return page1;
		}


		/* methods */


		/// <summary>
		/// Добавляет готовый информационный узел в конец коллекции корневых элементов.
		/// </summary>
		/// <param name="node">Добавляемый информационный узел.</param>
		public void AddTopNode(
			NodemapItem node)
		{
			if (node is not null)
				_topNodes.Add(node);
		}


		/// <summary>
		/// Вставляет информационный узел в коллекцию корневых элементов по строго указанному порядковому индексу.
		/// </summary>
		/// <param name="index">Порядковый индекс позиции вставки (начиная с 0).</param>
		/// <param name="node">Вставляемый информационный узел.</param>
		public void InsertTopNode(
			int index,
			NodemapItem node)
		{
			if (node is not null)
				_topNodes.Insert(index, node);
		}


		/// <summary>
		/// Выполняет рекурсивный обход текущего графа элементов и компилирует 
		/// высокопроизводительные неизменяемые словари (Frozen) для быстрого поиска узлов и страниц.
		/// </summary>
		public void BuildIndexes()
		{
			var nodesDict1 = new Dictionary<string, NodemapItem>(StringComparer.OrdinalIgnoreCase);
			var pagesDict1 = new Dictionary<string, PagemapItem>(StringComparer.OrdinalIgnoreCase);
			foreach (var node1 in _topNodes)
				_indexNodeRecursive(node1, nodesDict1, pagesDict1);
			NodesIndex = nodesDict1.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
			PagesIndex = pagesDict1.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
		}


		/* privates */


		private static void _indexNodeRecursive(
			NodemapItem node,
			Dictionary<string, NodemapItem> nodesDict,
			Dictionary<string, PagemapItem> pagesDict)
		{
			if (!string.IsNullOrEmpty(node.Name))
				nodesDict[node.Name] = node;
			foreach (var page1 in node._AllTopPages)
				_indexPageRecursive(page1, pagesDict);
			foreach (var childNode1 in node.ChildNodes)
				_indexNodeRecursive(childNode1, nodesDict, pagesDict);
		}


		private static void _indexPageRecursive(
			PagemapItem page,
			Dictionary<string, PagemapItem> pagesDict)
		{
			if (page.Path is not null)
				pagesDict[page.Path] = page;
			foreach (var childPage1 in page.ChildPages)
				_indexPageRecursive(childPage1, pagesDict);
		}

	}

}
