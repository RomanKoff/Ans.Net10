// rev 2026-10-22

using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Облегченное декларативное определение контентной страницы, применяемое для программных инъекций.
	/// </summary>
	public sealed class PagemapDef
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="PagemapDef"/>.
		/// </summary>
		/// <param name="target">Строка назначения страницы (ЧПУ-сегмент или путь).</param>
		/// <param name="face">Человекочитаемое название страницы.</param>
		/// <param name="hasNoIndex">Признак отключения индексации для страницы и её потомков.</param>
		/// <param name="childPages">Массив вложенных дочерних страниц.</param>
		public PagemapDef(
			string target,
			string? face,
			bool hasNoIndex = false,
			PagemapDef[]? childPages = null)
		{
			Target = target;
			Face = face;
			HasNoIndex = hasNoIndex;
			ChildPages = childPages;
		}

		/// <summary>
		/// Получает оригинальную строку назначения страницы.
		/// </summary>
		/// <returns>Строковый идентификатор назначения.</returns>
		public string Target { get; }

		/// <summary>
		/// Получает человекочитаемое наименование страницы.
		/// </summary>
		/// <returns>Название страницы либо <see langword="null"/>.</returns>
		public string? Face { get; }

		/// <summary>
		/// Получает значение, указывающее на отключение индексации потомков.
		/// </summary>
		/// <returns><see langword="true"/>, если индексация отключена; иначе — <see langword="false"/>.</returns>
		public bool HasNoIndex { get; }

		/// <summary>
		/// Получает массив вложенных дочерних страниц.
		/// </summary>
		/// <returns>Массив объектов <see cref="PagemapDef"/> либо <see langword="null"/>.</returns>
		public PagemapDef[]? ChildPages { get; }
	}



	/// <summary>
	/// Представляет отдельную контентную страницу карты сайта, связанную с конкретным информационным узлом.
	/// </summary>
	public sealed partial class PagemapItem
		: _MapItem_Proto
	{

		[GeneratedRegex("^([0-9a-z_-]+)$", RegexOptions.Compiled)]
		private static partial Regex _normalTargetRegex();

		private readonly List<PagemapItem> _childPages = [];


		/* ctors */


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
			bool hasNoIndex = false)
		{
			ArgumentNullException.ThrowIfNull(node);
			ArgumentNullException.ThrowIfNull(target);
			Node = node;
			Target = target;
			SetFace(string.IsNullOrEmpty(face) ? target == "start" ? null : target : face);
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


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="PagemapItem"/> на основе декларативного описания.
		/// </summary>
		/// <param name="node">Родительский информационный узел карты сайта.</param>
		/// <param name="def">Объект декларативного описания страницы.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="def"/> равен <see langword="null"/>.</exception>
		public PagemapItem(
			NodemapItem node,
			PagemapDef def)
			: this(node, def.Target, def.Face, def.HasNoIndex)
		{
			ArgumentNullException.ThrowIfNull(def);
			if (def.ChildPages is not null)
				foreach (var page1 in def.ChildPages)
					AddChildPage(new PagemapItem(node, page1));
		}


		/* overrides */


		/// <summary>
		/// Получает относительный ЧПУ-путь страницы, сформированный с учетом предков.
		/// </summary>
		/// <returns>Относительный ЧПУ-маршрут страницы в виде строки (например, "/catalog/subpage").</returns>
		public override string Path
			=> field ??= _buildPath();


		/// <summary>
		/// Формирует полный виртуальный URL-адрес страницы с учетом базового пути приложения.
		/// </summary>
		/// <param name="httpContext">Контекст текущего HTTP-запроса.</param>
		/// <returns>Абсолютный относительный путь к элементу навигации.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="httpContext"/> равен <see langword="null"/>.</exception>
		public override string GetUrl(
			HttpContext httpContext)
		{
			ArgumentNullException.ThrowIfNull(httpContext);
			return _url ??= Type switch
			{
				PagemapTypesEnum.InternalPath => $"{httpContext.GetVirtualPath()}{Target}",
				PagemapTypesEnum.ExternalUrl => Target,
				_ => $"{httpContext.GetVirtualPath()}{Path}"
			};
		}
		private string? _url;


		/* readonly properties */


		/// <summary>
		/// Получает родительский информационный узел текущей страницы.
		/// </summary>
		/// <returns>Экземпляр связанного навигационного узла <see cref="NodemapItem"/>.</returns>
		public NodemapItem Node { get; }


		/// <summary>
		/// Получает оригинальную строку назначения страницы.
		/// </summary>
		/// <returns>Строковый идентификатор назначения (путь или URL).</returns>
		public string Target { get; }


		/// <summary>
		/// Получает значение, указывающее на отсутствие индексной страницы в потомках данной страницы.
		/// </summary>
		/// <returns><see langword="true"/>, если индексные страницы потомков отключены; иначе — <see langword="false"/>.</returns>
		public bool HasNoIndex { get; }


		/// <summary>
		/// Получает системное ЧПУ-имя страницы, если тип относится к <see cref="PagemapTypesEnum.Page"/>.
		/// </summary>
		/// <returns>Имя ЧПУ-сегмента страницы либо <see langword="null"/>.</returns>
		public string? Name { get; }


		/// <summary>
		/// Получает семантический тип страницы контента, определяющий её поведение в структуре навигации.
		/// </summary>
		/// <value>
		/// Одно из значений перечисления <see cref="PagemapTypesEnum"/>, указывающее, является ли страница 
		/// изолированным элементом, каталогом контента, внутренней ссылкой или внешним URL.
		/// </value>
		/// <returns>Текущий тип страницы из перечисления <see cref="PagemapTypesEnum"/>.</returns>
		public PagemapTypesEnum Type { get; private set; }


		/// <summary>
		/// Получает ссылку на родительскую страницу в иерархическом дереве контента.
		/// </summary>
		/// <returns>Экземпляр родительской страницы <see cref="PagemapItem"/> либо <see langword="null"/>, если страница находится на верхнем уровне узла.</returns>
		public PagemapItem? ParentPage { get; private set; }


		/// <summary>
		/// Получает коллекцию прямых дочерних страниц.
		/// </summary>
		/// <returns>Коллекция только для чтения, содержащая вложенные страницы <see cref="PagemapItem"/>.</returns>
		public IReadOnlyCollection<PagemapItem> ChildPages { get; }


		/* methods */


		public void AddChildPage(
			PagemapItem child)
		{
			if (Type is not PagemapTypesEnum.Page and not PagemapTypesEnum.Catalog)
				return;
			Type = PagemapTypesEnum.Catalog;
			child.ParentPage = this;
			_childPages.Add(child);
		}


		/* functions */


		/// <summary>
		/// Получает ленивое перечисление всех родительских страниц по цепочке навигации вверх до корня. 
		/// Возвращает <see langword="null"/>, если страница находится на корневом уровне информационного узла.
		/// </summary>
		/// <returns>Последовательность вышестоящих страниц <see cref="PagemapItem"/> либо <see langword="null"/>.</returns>
		public IEnumerable<PagemapItem>? GetParentPages()
		{
			if (ParentPage is null)
				return null;
			return _enumerateParentPages();
		}


		/* privates */


		private string _buildPath()
		{
			if (Name is not { Length: > 0 })
				return string.Empty;
			var parts1 = new List<string>();
			var ancestors1 = GetParentPages();
			if (ancestors1 is not null)
				foreach (var ancestor1 in ancestors1)
					if (ancestor1.Name is { Length: > 0 }
						&& !string.Equals(ancestor1.Name, LibWebConsts.INDEX_PAGE_NAME, StringComparison.OrdinalIgnoreCase))
						parts1.Insert(0, ancestor1.Name);
			if (!string.Equals(Name, LibWebConsts.INDEX_PAGE_NAME, StringComparison.OrdinalIgnoreCase))
				parts1.Add(Name);
			var nodePath1 = Node.Path;
			if (parts1 is [])
				return nodePath1 is { Length: 1 }
					? "/" : nodePath1;
			var pagesPath1 = string.Join('/', parts1);
			return nodePath1 is { Length: 1 }
				? $"/{pagesPath1}"
				: $"{nodePath1}/{pagesPath1}";
		}


		private IEnumerable<PagemapItem> _enumerateParentPages()
		{
			var current1 = ParentPage;
			while (current1 is not null)
			{
				yield return current1;
				current1 = current1.ParentPage;
			}
		}

	}



	/// <summary>
	/// Перечисление семантических типов страниц.
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

}