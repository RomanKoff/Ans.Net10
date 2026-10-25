// rev 2026-10-23

using Ans.Net10.Common;
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


		/* ctors */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NodemapItem"/>.
		/// </summary>
		/// <param name="target">Строка назначения (идентификатор узла, внутренний или внешний URL).</param>
		/// <param name="face">Отображаемое человекочитаемое наименование узла.</param>
		/// <param name="display">
		/// Декларативное правило видимости в формате <c>"теги_показа|теги_скрытия"</c> (например, <c>"premium,admin|archive"</c>, <c>"manager"</c> или <c>"|guest"</c>). 
		/// Переданная строка автоматически парсится в высокопроизводительные nullable-массивы тегов.
		/// </param>		
		/// <param name="passthroughs">
		/// Декларативное правило сквозного поведения (прозрачности) в формате <c>"теги_включения|теги_исключения"</c> (например, <c>"system|debug-off"</c>, <c>"mobile"</c> или <c>"|desktop"</c>). 
		/// Переданная строка автоматически парсится в высокопроизводительные nullable-массивы тегов активации и отключения сквозного режима.
		/// </param>
		///  <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="face"/> равен <see langword="null"/>.</exception>
		public NodemapItem(
			string? target,
			string face,
			string? display = null,
			string? passthroughs = null)
		{
			ArgumentNullException.ThrowIfNull(face);
			Target = target;
			SetFace(face);
			ChildNodes = _childNodes.AsReadOnly();
			(_DisplayOnTags, _DisplayOffTags) = SuppValues.ParseRule(display);
			(_PassthroughsOnTags, _PassthroughsOffTags) = SuppValues.ParseRule(passthroughs);
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
				AddTopPage(startPage1);
				StartPage = startPage1;
			}
			else
			{
				Type = NodemapTypesEnum.ExternalUrl;
				Name = null;
			}
			AllTopPages = _topPages.AsReadOnly();
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NodemapItem"/> на основе декларативного описания.
		/// </summary>
		/// <param name="def">Объект декларативного описания узла.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="def"/> равен <see langword="null"/>.</exception>
		public NodemapItem(
			NodemapDef def)
			: this(def.Target, def.Face, def.Display, def.Passthroughs)
		{
			ArgumentNullException.ThrowIfNull(def);
			if (def.TopPages is not null)
				foreach (var page1 in def.TopPages)
					AddTopPage(new PagemapItem(this, page1));
			if (def.ChildNodes is not null)
				foreach (var node1 in def.ChildNodes)
					AddChildNode(new NodemapItem(node1));
		}


		/* overrides */


		/// <summary>
		/// Получает относительный ЧПУ-путь узла.
		/// </summary>
		/// <returns>Относительный ЧПУ-путь в формате строки (например, "/catalog").</returns>
		public override string Path
			=> field ??= _buildPath();


		/// <summary>
		/// Формирует полный виртуальный URL-адрес узла с учетом базового пути приложения.
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
				NodemapTypesEnum.Node => $"{httpContext.GetVirtualPath()}{Path}",
				NodemapTypesEnum.Group => string.Empty,
				NodemapTypesEnum.InternalPath => $"{httpContext.GetVirtualPath()}{Target}",
				_ => Target ?? string.Empty
			};
		}
		private string? _url = null;


		/* readonly properties */


		/// <summary>
		/// Получает оригинальную строку назначения узла.
		/// </summary>
		/// <returns>Строка назначения (идентификатор, относительный или абсолютный URL) либо <see langword="null"/>.</returns>
		public string? Target { get; }


		/// <summary>
		/// Получает очищенное системное имя узла, если он относится к типу <see cref="NodemapTypesEnum.Node"/>.
		/// </summary>
		/// <returns>Строковый системный идентификатор узла либо <see langword="null"/>.</returns>
		public string? Name { get; }


		/// <summary>
		/// Получает семантический тип узла.
		/// </summary>
		/// <returns>Значение перечисления <see cref="NodemapTypesEnum"/>, определяющее поведение узла.</returns>
		public NodemapTypesEnum Type { get; }


		/// <summary>
		/// Получает выделенную стартовую (индексную) контентную страницу данного информационного узла.
		/// Возвращает <see langword="null"/>, если узел не относится к типу <see cref="NodemapTypesEnum.Node"/>.
		/// </summary>
		/// <returns>Объект страницы <see cref="PagemapItem"/> либо <see langword="null"/>.</returns>
		public PagemapItem? StartPage { get; }


		/// <summary>
		/// Получает ссылку на родительский узел-контейнер.
		/// </summary>
		/// <returns>Экземпляр родительского узла <see cref="NodemapItem"/> либо <see langword="null"/>.</returns>
		public NodemapItem? ParentNode { get; internal set; }


		/// <summary>
		/// Получает коллекцию прямых дочерних узлов.
		/// </summary>
		/// <returns>Коллекция только для чтения, содержащая дочерние узлы <see cref="NodemapItem"/>.</returns>
		public IReadOnlyCollection<NodemapItem> ChildNodes { get; }


		/// <summary>
		/// Получает коллекцию корневых страниц, принадлежащих данному узлу (исключая техническую стартовую страницу).
		/// </summary>
		/// <returns>Коллекция только для чтения, содержащая значимые страницы <see cref="PagemapItem"/>.</returns>
		public IReadOnlyCollection<PagemapItem> TopPages
			=> _pagesCache1 ??= [.. _topPages.Where(
					x => !string.Equals(
						x.Name, LibWebConsts.INDEX_PAGE_NAME, StringComparison.OrdinalIgnoreCase))];


		/// <summary>
		/// Получает коллекцию всех корневых страниц, принадлежащих данному узлу.
		/// </summary>
		/// <returns>Коллекция только для чтения, содержащая вложенные страницы <see cref="PagemapItem"/>.</returns>
		public IReadOnlyCollection<PagemapItem> AllTopPages { get; }


		/// <summary>
		/// Получает признак наличия дочерних узлов у текущего элемента карты узлов.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если коллекция дочерних узлов содержит элементы; иначе — <see langword="false"/>.</value>
		/// <returns><see langword="true"/>, если есть дочерние узлы; иначе — <see langword="false"/>.</returns>
		public bool HasChildNodes
			=> ChildNodes is { Count: > 0 };


		/// <summary>
		/// Получает признак наличия страниц у текущего элемента карты узлов.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если коллекция основных страниц содержит элементы; иначе — <see langword="false"/>.</value>
		/// <returns><see langword="true"/>, если есть основные страницы; иначе — <see langword="false"/>.</returns>
		public bool HasPages
			=> TopPages is { Count: > 0 };


		/* methods */


		/// <summary>
		/// Добавляет готовый дочерний информационный узел в текущую ветку дерева навигации.
		/// </summary>
		/// <param name="child">Экземпляр добавляемого дочернего информационного узла <see cref="NodemapItem"/>.</param>
		public void AddChildNode(
			NodemapItem child)
		{
			child.ParentNode = this;
			_childNodes.Add(child);
		}


		/// <summary>
		/// Добавляет страницу верхнего уровня в коллекцию страниц текущего информационного узла.
		/// </summary>
		/// <param name="page">Экземпляр добавляемой контентной страницы <see cref="PagemapItem"/>.</param>
		public void AddTopPage(
			PagemapItem page)
		{
			_topPages.Add(page);
		}


		/* functions */


		/// <summary>
		/// Получает ленивое перечисление всех вышестоящих родительских узлов по цепочке вверх до корня. 
		/// Возвращает <see langword="null"/>, если текущий узел находится на самом верхнем уровне.
		/// </summary>
		/// <returns>Последовательность родительских узлов <see cref="NodemapItem"/> либо <see langword="null"/>.</returns>
		public IEnumerable<NodemapItem>? GetParentNodes()
		{
			if (ParentNode is null)
				return null;
			return _enumerateParentNodes();
		}


		/* internals */


		internal IReadOnlyCollection<PagemapItem> _AllTopPages
			=> _topPages.AsReadOnly();


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