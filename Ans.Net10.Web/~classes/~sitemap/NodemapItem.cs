// rev 2026-10-20

using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Представляет информационный узел карты сайта, определяющий элемент структуры навигации.
	/// </summary>
	public sealed partial class NodemapItem
		: _MapItem_Proto
	{

		[GeneratedRegex("^([0-9a-z_-]+)$", RegexOptions.Compiled)]
		private static partial Regex _normalTargetRegex();

		private readonly List<NodemapItem> _childNodes = [];
		private readonly List<PagemapItem> _topPages = [];
		private IReadOnlyCollection<PagemapItem>? _pagesCache1;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NodemapItem"/>.
		/// </summary>
		/// <param name="target">Строка назначения (идентификатор узла, внутренний или внешний URL).</param>
		/// <param name="face">Отображаемое человекочитаемое наименование узла.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="face"/> равен <see langword="null"/>.</exception>
		public NodemapItem(
			string? target,
			string face)
		{
			Target = target;
			SetFace(face);
			ChildNodes = _childNodes.AsReadOnly();
			if (string.IsNullOrEmpty(target))
			{
				Type = NodemapTypesEnum.Group;
				Name = null;
			}
			else if (target.StartsWith('/'))
			{
				Type = NodemapTypesEnum.InternalPath;
				Name = null;
			}
			else if (_normalTargetRegex().IsMatch(target))
			{
				Type = NodemapTypesEnum.Node;
				Name = target;
				var startPage1 = new PagemapItem(this, LibWebConsts.INDEX_PAGE_NAME, null, false);
				_AddTopPage(startPage1);
				StartPage = startPage1;
			}
			else
			{
				Type = NodemapTypesEnum.ExternalUrl;
				Name = null;
			}
		}


		/* overrides */


		/// <summary>
		/// Получает относительный ЧПУ-путь узла.
		/// </summary>
		public override string Path
			=> field ??= _buildPath();


		/// <summary>
		/// Формирует полный виртуальный URL-адрес узла с учетом базового пути приложения.
		/// </summary>
		/// <param name="httpContext">Контекст текущего HTTP-запроса.</param>
		/// <returns>Абсолютный относительный путь к элементу навигации.</returns>
		public override string GetUrl(
			HttpContext httpContext)
		{
			ArgumentNullException.ThrowIfNull(httpContext);
			return _url ??= Type switch
			{
				NodemapTypesEnum.Node => $"{httpContext.GetVirtualPath()}{Path}",
				NodemapTypesEnum.Group => string.Empty,
				NodemapTypesEnum.InternalPath => $"{httpContext.GetVirtualPath()}{Target}",
				_ => Target ?? string.Empty
			};
		}


		/* readonly properties */


		/// <summary>
		/// Получает оригинальную строку назначения узла.
		/// </summary>
		public string? Target { get; }


		/// <summary>
		/// Получает очищенное системное имя узла, если он относится к типу <see cref="NodemapTypesEnum.Node"/>.
		/// </summary>
		public string? Name { get; }


		/// <summary>
		/// Получает семантический тип узла.
		/// </summary>
		public NodemapTypesEnum Type { get; }


		/// <summary>
		/// Получает выделенную стартовую (индексную) контентную страницу данного информационного узла.
		/// Возвращает <see langword="null"/>, если узел не относится к типу <see cref="NodemapTypesEnum.Node"/>.
		/// </summary>
		public PagemapItem? StartPage { get; }


		/// <summary>
		/// Получает ссылку на родительский узел-контейнер.
		/// </summary>
		public NodemapItem? ParentNode { get; internal set; }


		/// <summary>
		/// Получает коллекцию прямых дочерних узлов.
		/// </summary>
		public IReadOnlyCollection<NodemapItem> ChildNodes { get; }


		/// <summary>
		/// Получает коллекцию корневых страниц, принадлежащих данному узлу (исключая техническую стартовую страницу).
		/// </summary>
		public IReadOnlyCollection<PagemapItem> TopPages
			=> _pagesCache1 ??= _topPages
				.Where(p => !string.Equals(p.Name, LibWebConsts.INDEX_PAGE_NAME, StringComparison.OrdinalIgnoreCase))
				.ToList()
				.AsReadOnly();


		/// <summary>
		/// Внутренний метод доступа ко ВСЕМ страницам узла (включая start) для построения индексов в SitemapBuilder.
		/// </summary>
		internal IReadOnlyCollection<PagemapItem> _AllTopPages
			=> _topPages.AsReadOnly();


		/* functions */


		/// <summary>
		/// Получает ленивое перечисление всех вышестоящих родительских узлов по цепочке вверх до корня. 
		/// Возвращает <see langword="null"/>, если текущий узел находится на самом верхнем уровне.
		/// </summary>
		public IEnumerable<NodemapItem>? GetParentNodes()
		{
			if (ParentNode is null)
				return null;
			return _enumerateParentNodes();
		}


		/* internals */


		internal void _AddChildNode(
			NodemapItem child)
		{
			child.ParentNode = this;
			_childNodes.Add(child);
		}


		internal void _AddTopPage(
			PagemapItem page)
		{
			_topPages.Add(page);
		}


		/* privates */
		

		private string _buildPath()
		{
			if (string.IsNullOrEmpty(Name) || Name == LibWebConsts.INDEX_NODE_NAME)
				return "/";
			return $"/{Name}";
		}


		private IEnumerable<NodemapItem> _enumerateParentNodes()
		{
			var current1 = ParentNode;
			while (current1 is not null)
			{
				yield return current1;
				current1 = current1.ParentNode;
			}
		}

	}



	/// <summary>
	/// Перечисление семантических типов узлов.
	/// </summary>
	public enum NodemapTypesEnum
	{
		/// <summary>
		/// Стандартный функциональный узел структуры. Обладает выделенным системным именем ЧПУ 
		/// и выступает в роли физического контейнера для собственной иерархии страниц.
		/// </summary>
		Node,

		/// <summary>
		/// Визуальный узел группировки. Не обладает собственной ссылкой назначения или системным именем. 
		/// Служит исключительно в качестве декоративного контейнера/заголовка для объединения дочерних узлов в меню.
		/// </summary>
		Group,

		/// <summary>
		/// Ссылочный узел, указывающий на произвольный внутренний относительный путь текущего сайта. 
		/// Не участвует в автоматическом иерархическом построении ЧПУ-маршрутов страниц.
		/// </summary>
		InternalPath,

		/// <summary>
		/// Внешний узел-ссылка, перенаправляющий навигацию на сторонний интернет-ресурс по абсолютному URL-адресу.
		/// Не участвует в автоматическом иерархическом построении ЧПУ-маршрутов страниц.
		/// </summary>
		ExternalUrl
	}

}