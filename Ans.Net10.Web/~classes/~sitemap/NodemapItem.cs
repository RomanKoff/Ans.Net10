// rev 2026-10-16

using System.Text.RegularExpressions;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Перечисление семантических типов организации информационных узлов в рамках карты сайта.
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



	/// <summary>
	/// Представляет информационный узел карты сайта, определяющий элемент структуры навигации.
	/// </summary>
	public sealed partial class NodemapItem
	{

		[GeneratedRegex("^([0-9a-z_-]+)$", RegexOptions.Compiled)]
		private static partial Regex _normalTargetRegex();

		private readonly List<NodemapItem> _childNodes = [];
		private readonly List<PagemapItem> _topPages = [];


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
			Face = face ?? throw new ArgumentNullException(nameof(face));
			ChildNodes = _childNodes.AsReadOnly();
			TopPages = _topPages.AsReadOnly();
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
			}
			else
			{
				Type = NodemapTypesEnum.ExternalUrl;
				Name = null;
			}
		}


		/* readonly properties */


		/// <summary>
		/// Получает оригинальную строку назначения узла.
		/// </summary>
		public string? Target { get; }


		/// <summary>
		/// Получает определение заголовка узла.
		/// </summary>
		public string Face { get; }


		/// <summary>
		/// Получает очищенное системное имя узла, если он относится к типу <see cref="NodemapTypesEnum.Node"/>.
		/// </summary>
		public string? Name { get; }


		/// <summary>
		/// Получает семантический тип узла.
		/// </summary>
		public NodemapTypesEnum Type { get; }


		/// <summary>
		/// Получает относительный ЧПУ-путь узла. Возвращает <see langword="null"/>, если имя отсутствует.
		/// </summary>
		public string? Path
			=> field ??= _buildPath();


		/// <summary>
		/// Получает ссылку на родительский узел-контейнер.
		/// </summary>
		public NodemapItem? ParentNode { get; internal set; }


		/// <summary>
		/// Получает коллекцию прямых дочерних узлов.
		/// </summary>
		public IReadOnlyCollection<NodemapItem> ChildNodes { get; }


		/// <summary>
		/// Получает коллекцию корневых страниц, принадлежащих данному узлу.
		/// </summary>
		public IReadOnlyCollection<PagemapItem> TopPages { get; }


		/// <summary>
		/// Получает ленивое перечисление всех вышестоящих родительских узлов по цепочке вверх до корня.
		/// </summary>
		public IEnumerable<NodemapItem> ParentNodes
		{
			get
			{
				var current1 = ParentNode;
				while (current1 is not null)
				{
					yield return current1;
					current1 = current1.ParentNode;
				}
			}
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


		private string? _buildPath()
		{
			if (string.IsNullOrEmpty(Name))
				return null;
			if (Name == LibWebConsts.INDEX_NODE_NAME)
				return string.Empty;
			return Name;
		}

	}

}