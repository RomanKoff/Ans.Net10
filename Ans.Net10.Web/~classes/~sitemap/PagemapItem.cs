// rev 2026-10-16

using System.Text.RegularExpressions;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Перечисление семантических типов контентных страниц в рамках информационного узла карты сайта.
	/// </summary>
	public enum PagemapTypesEnum
	{
		/// <summary>
		/// Стандартная изолированная контентная страница. Обладает системным ЧПУ-именем, 
		/// не содержит вложенных дочерних страниц.
		/// </summary>
		Page,

		/// <summary>
		/// Каталог (структурный раздел). Является контентной страницей, которая дополнительно 
		/// содержит вложенную иерархию подчиненных дочерних страниц.
		/// </summary>
		Catalog,

		/// <summary>
		/// Ссылочный элемент страницы, указывающий на произвольный внутренний относительный путь сайта. 
		/// Не генерирует собственный иерархический ЧПУ-путь на основе предков.
		/// </summary>
		InternalPath,

		/// <summary>
		/// Внешний элемент-ссылка, перенаправляющий пользователя на сторонний интернет-ресурс по абсолютному URL-адресу.
		/// Не генерирует собственный иерархический ЧПУ-путь на основе предков.
		/// </summary>
		ExternalUrl
	}



	/// <summary>
	/// Представляет отдельную контентную страницу карты сайта, связанную с конкретным информационным узлом.
	/// </summary>
	public sealed partial class PagemapItem
	{

		[GeneratedRegex("^([0-9a-z_-]+)$", RegexOptions.Compiled)]
		private static partial Regex _normalTargetRegex();

		private readonly List<PagemapItem> _childPages = [];


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="PagemapItem"/>.
		/// </summary>
		/// <param name="node">Родительский информационный узел, к которому относится страница.</param>
		/// <param name="target">Строка назначения страницы (ЧПУ-имя, внутренний или внешний URL).</param>
		/// <param name="face">Отображаемое наименование страницы. Если пустой — подставляется значение назначения.</param>
		/// <param name="hasNoIndex">Признак отсутствия индексной страницы в потомках.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="node"/> или <paramref name="target"/> равен <see langword="null"/>.</exception>
		public PagemapItem(
			NodemapItem node,
			string target,
			string? face,
			bool hasNoIndex)
		{
			Node = node ?? throw new ArgumentNullException(nameof(node));
			Target = target ?? throw new ArgumentNullException(nameof(target));
			Face = string.IsNullOrEmpty(face)
				? Target : face;
			HasNoIndex = hasNoIndex;
			if (target.StartsWith('/'))
			{
				Type = PagemapTypesEnum.InternalPath;
				Name = null;
			}
			else if (_normalTargetRegex().IsMatch(target))
			{
				Type = PagemapTypesEnum.Page;
				Name = target;
			}
			else
			{
				Type = PagemapTypesEnum.ExternalUrl;
				Name = null;
			}
			ChildPages = _childPages.AsReadOnly();
		}


		/* readonly properties */


		/// <summary>
		/// Получает родительский информационный узел текущей страницы.
		/// </summary>
		public NodemapItem Node { get; }


		/// <summary>
		/// Получает оригинальную строку назначения страницы.
		/// </summary>
		public string Target { get; }


		/// <summary>
		/// Получает определение заголовка страницы.
		/// </summary>
		public string Face { get; }


		/// <summary>
		/// Получает значение, указывающее на отсутсвие индексной страницы в потомках данной страницы.
		/// </summary>
		public bool HasNoIndex { get; }


		/// <summary>
		/// Получает системное ЧПУ-имя страницы, если тип относится к <see cref="PagemapTypesEnum.Page"/>.
		/// </summary>
		public string? Name { get; }


		/// <summary>
		/// Получает семантический тип страницы.
		/// </summary>
		public PagemapTypesEnum Type { get; private set; }


		/// <summary>
		/// Получает относительный ЧПУ-путь страницы, сформированный с учетом предков. Возвращает <see langword="null"/>, если имя отсутствует.
		/// </summary>
		public string? Path
			=> field ??= _buildPath();


		/// <summary>
		/// Получает ссылку на родительскую страницу в иерархическом дереве контента.
		/// </summary>
		public PagemapItem? ParentPage { get; private set; }


		/// <summary>
		/// Получает коллекцию прямых дочерних страниц.
		/// </summary>
		public IReadOnlyCollection<PagemapItem> ChildPages { get; }


		/// <summary>
		/// Получает ленивое перечисление всех родительских страниц по цепочке навигации вверх до корня.
		/// </summary>
		public IEnumerable<PagemapItem> ParentPages
		{
			get
			{
				var current1 = ParentPage;
				while (current1 is not null)
				{
					yield return current1;
					current1 = current1.ParentPage;
				}
			}
		}


		/* internals */


		internal void _AddChildPage(
			PagemapItem child)
		{
			if (Type is not PagemapTypesEnum.Page and not PagemapTypesEnum.Catalog)
				return;
			Type = PagemapTypesEnum.Catalog;
			child.ParentPage = this;
			_childPages.Add(child);
		}


		/* privates */


		private string? _buildPath()
		{
			if (Name is null)
				return null;
			var parts1 = new List<string>();
			foreach (var ancestor1 in ParentPages)
				if (!string.IsNullOrEmpty(ancestor1.Name))
					parts1.Insert(0, ancestor1.Name);
			parts1.Add(Name);
			if (!string.IsNullOrEmpty(Node.Path))
				parts1.Insert(0, Node.Path);
			return string.Join('/', parts1);
		}

	}

}