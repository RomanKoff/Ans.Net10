namespace Ans.Net10.Web.Sitemap
{

	/// <summary>
	/// Описывает данные информационного узла в карте сайта.
	/// </summary>
	public sealed class NodeItem
	{

		private readonly List<NodeItem> _childNodes = [];
		private readonly List<PageItem> _topPages = [];


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NodeItem"/>.
		/// </summary>
		/// <param name="target">Целевой идентификатор или адрес узла.</param>
		/// <param name="face">Сырая строка автотипографики и заголовков.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="target"/> или <paramref name="face"/> равен <see langword="null"/>.</exception>
		public NodeItem(
			string target,
			string face)
		{
			Target = target ?? throw new ArgumentNullException(nameof(target));
			Face = face ?? throw new ArgumentNullException(nameof(face));
			Name = Target;
			ChildNodes = _childNodes.AsReadOnly();
			TopPages = _topPages.AsReadOnly();
		}


		/* readonly properties */


		/// <summary>
		/// Получает имя, уникально идентифицирующее узел в карте узлов.
		/// </summary>
		/// <value>Строковый идентификатор или <see langword="null"/> для ссылочных узлов.</value>
		public string? Name { get; }


		/// <summary>
		/// Получает целевой идентификатор или адрес ссылки узла.
		/// </summary>
		public string Target { get; }


		/// <summary>
		/// Получает сырую строку автотипографики и заголовков.
		/// </summary>
		public string Face { get; }


		/// <summary>
		/// Получает родительский информационный узел текущего элемента.
		/// </summary>
		/// <value>Ссылка на родительский узел или <see langword="null"/>, если узел является корневым.</value>
		public NodeItem? ParentNode { get; internal set; }


		/// <summary>
		/// Получает коллекцию прямых подчиненных (дочерних) информационных узлов.
		/// </summary>
		public IReadOnlyCollection<NodeItem> ChildNodes { get; }


		/// <summary>
		/// Получает коллекцию корневых страниц, принадлежащих данному узлу.
		/// </summary>
		public IReadOnlyCollection<PageItem> TopPages { get; }


		/// <summary>
		/// Получает ленивое перечисление всех вышестоящих родительских узлов по цепочке вверх до корня дерева.
		/// </summary>
		public IEnumerable<NodeItem> ParentNodes
		{
			get
			{
				var current1 = ParentNode;
				while (current1 != null)
				{
					yield return current1;
					current1 = current1.ParentNode;
				}
			}
		}


		/* methods */


		internal void AddChildNode(
			NodeItem child)
		{
			child.ParentNode = this;
			_childNodes.Add(child);
		}


		internal void AddTopPage(
			PageItem page)
		{
			_topPages.Add(page);
		}

	}

}
