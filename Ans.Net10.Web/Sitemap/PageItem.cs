namespace Ans.Net10.Web.Sitemap
{

	/// <summary>
	/// Описывает данные контентной страницы в карте сайта.
	/// </summary>
	public sealed class PageItem
	{

		private readonly List<PageItem> _childPages = [];
		private string? _path;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="PageItem"/>.
		/// </summary>
		/// <param name="target">Сегмент ЧПУ-пути или адрес ссылки.</param>
		/// <param name="face">Сырая строка автотипографики и заголовков страницы.</param>
		/// <param name="hasStart">Признак наличия индексной страницы для текущего уровня.</param>
		/// <param name="node">Информационный узел, к которому жестко привязана страница.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="target"/>, <paramref name="face"/> или <paramref name="node"/> равен <see langword="null"/>.</exception>
		public PageItem(
			string target,
			string face,
			bool hasStart,
			NodeItem node)
		{
			Target = target ?? throw new ArgumentNullException(nameof(target));
			Face = face ?? throw new ArgumentNullException(nameof(face));
			Name = Target;
			HasStart = hasStart;
			Node = node ?? throw new ArgumentNullException(nameof(node));
			ChildPages = _childPages.AsReadOnly();
		}


		/* readonly properties */


		/// <summary>
		/// Получает сегмент ЧПУ-пути или адрес ссылки.
		/// </summary>
		public string Target { get; }


		/// <summary>
		/// Получает имя, идентифицирующее страницу в пределах ветки.
		/// </summary>
		/// <value>Строковый идентификатор или <see langword="null"/> для ссылочных страниц.</value>
		public string? Name { get; }


		/// <summary>
		/// Получает сырую строку автотипографики и заголовков страницы.
		/// </summary>
		public string Face { get; }


		/// <summary>
		/// Получает признак наличия индексной страницы для текущего уровня.
		/// </summary>
		public bool HasStart { get; }


		/// <summary>
		/// Получает информационный узел, к которому жестко привязана текущая страница.
		/// </summary>
		public NodeItem Node { get; }


		/// <summary>
		/// Получает родительскую контентную страницу в иерархическом дереве текущего узла.
		/// </summary>
		/// <value>Ссылка на родительскую страницу или <see langword="null"/>, если страница является корневой в рамках узла.</value>
		public PageItem? ParentPage { get; internal set; }


		/// <summary>
		/// Получает коллекцию прямых подчиненных (дочерних) контентных страниц.
		/// </summary>
		public IReadOnlyCollection<PageItem> ChildPages { get; }


		/// <summary>
		/// Получает полный путь страницы, сформированный на основе ее имени и имен страниц-предков.
		/// </summary>
		/// <remarks>
		/// В соответствии с правилами .NET 10 используется оптимизированная интерполяция строк.
		/// Если страница ссылочная (имя отсутствует), возвращает <see langword="null"/>.
		/// </remarks>
		public string? Path
			=> _path ??= _buildPath();


		/// <summary>
		/// Получает ленивое перечисление всех вышестоящих родительских страниц по цепочке вверх до корня текущего узла.
		/// </summary>
		public IEnumerable<PageItem> ParentPages
		{
			get
			{
				var current1 = ParentPage;
				while (current1 != null)
				{
					yield return current1;
					current1 = current1.ParentPage;
				}
			}
		}


		/* methods */


		internal void AddChildPage(
			PageItem child)
		{
			child.ParentPage = this;
			_childPages.Add(child);
		}


		/* privates */


		private string? _buildPath()
		{
			if (string.IsNullOrEmpty(Name))
				return null;
			var parts1 = new List<string>();
			foreach (var ancestor1 in ParentPages)
				if (!string.IsNullOrEmpty(ancestor1.Name))
					parts1.Insert(0, ancestor1.Name);
			parts1.Add(Name);
			return string.Join('/', parts1);
		}

	}

}
