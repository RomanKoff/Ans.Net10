// rev 2026-10-22

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
		/// <returns>Коллекция только для чтения, содержащая корневые узлы <see cref="NodemapItem"/>.</returns>
		public IReadOnlyCollection<NodemapItem> TopNodes { get; }


		/// <summary>
		/// Получает индексный словарь информационных узлов карты сайта.
		/// </summary>
		/// <returns>Оптимизированный для чтения замороженный словарь, сопоставляющий системные имена с объектами <see cref="NodemapItem"/>.</returns>
		public FrozenDictionary<string, NodemapItem> NodesIndex { get; private set; }
			= FrozenDictionary<string, NodemapItem>.Empty;


		/// <summary>
		/// Получает индексный словарь информационных страниц карты сайта.
		/// </summary>
		/// <returns>Оптимизированный для чтения замороженный словарь, сопоставляющий полные ЧПУ-маршруты с объектами <see cref="PagemapItem"/>.</returns>
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
		/// Программно преобразует и добавляет корневой узел на основе его декларативного описания в конец коллекции.
		/// </summary>
		/// <param name="nodeDef">Декларативное описание добавляемого узла.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="nodeDef"/> равен <see langword="null"/>.</exception>
		public void AddTopNodeDef(
			NodemapDef nodeDef)
		{
			ArgumentNullException.ThrowIfNull(nodeDef);
			var node1 = new NodemapItem(nodeDef);
			AddTopNode(node1);
		}


		/// <summary>
		/// Программно добавляет корневой информационный узел в карту сайта.
		/// </summary>
		/// <param name="node">Добавляемый информационный узел <see cref="NodemapItem"/>.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="node"/> равен <see langword="null"/>.</exception>
		public void AddTopNode(
			NodemapItem node)
		{
			ArgumentNullException.ThrowIfNull(node);
			_topNodes.Add(node);
		}


		/// <summary>
		/// Вставляет информационный узел в коллекцию корневых элементов по строго указанному порядковому индексу.
		/// </summary>
		/// <param name="index">Порядковый индекс позиции вставки (начиная с 0).</param>
		/// <param name="node">Вставляемый информационный узел.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="node"/> равен <see langword="null"/>.</exception>
		public void InsertTopNode(
			int index,
			NodemapItem node)
		{
			ArgumentNullException.ThrowIfNull(node);
			_topNodes.Insert(index, node);
		}


		/// <summary>
		/// Программно вставляет дочерний узел в уже существующий родительский узел карты сайта.
		/// </summary>
		/// <param name="parentTarget">Идентификатор или путь целевого родительского узла.</param>
		/// <param name="childNode">Новый дочерний узел <see cref="NodemapItem"/>.</param>
		/// <returns>Значение <see langword="true"/>, если родительский узел найден и инъекция успешна; иначе — <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="childNode"/> равен <see langword="null"/>.</exception>
		public bool InjectChildNode(
			string parentTarget,
			NodemapItem childNode)
		{
			ArgumentNullException.ThrowIfNull(childNode);
			if (NodesIndex.TryGetValue(parentTarget, out var parentNode1))
			{
				parentNode1.AddChildNode(childNode);
				return true;
			}
			return false;
		}


		/// <summary>
		/// Программно вставляет контентную страницу в существующий информационный узел графа.
		/// </summary>
		/// <param name="nodeTarget">Идентификатор информационного узла, в который добавляется страница.</param>
		/// <param name="page">Новая страница контента <see cref="PagemapItem"/>.</param>
		/// <returns>Значение <see langword="true"/>, если целевой узел найден и страница добавлена; иначе — <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="page"/> равен <see langword="null"/>.</exception>
		public bool InjectPageToNode(
			string nodeTarget,
			PagemapItem page)
		{
			ArgumentNullException.ThrowIfNull(page);
			if (NodesIndex.TryGetValue(nodeTarget, out var node1))
			{
				node1.AddTopPage(page);
				return true;
			}
			return false;
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
			NodesIndex = nodesDict1
				.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
			PagesIndex = pagesDict1
				.Where(x => x.Key is not { Length: 0 })
				.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
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
