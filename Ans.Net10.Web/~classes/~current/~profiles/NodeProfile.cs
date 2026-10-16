// rev 2026-10-14

namespace Ans.Net10.Web
{

	/// <summary>
	/// Перечисление семантических типов профилей узлов.
	/// </summary>
	public enum NodeProfileType
	{
		/// <summary>
		/// Стандартный узел, имеющий физическое представление и контент на сайте.
		/// </summary>
		Node,

		/// <summary>
		/// Узел группировки. Служит исключительно в качестве визуального контейнера для других узлов в навигации и не имеет собственной ссылки.
		/// </summary>
		Group,

		/// <summary>
		/// Внутренняя ссылка. Перенаправляет навигацию на другой узел или страницу этого же сайта.
		/// </summary>
		InternalLink,

		/// <summary>
		/// Внешняя ссылка. Ведет на сторонний интернет-ресурс.
		/// </summary>
		ExternalLink
	}



	/// <summary>
	/// Описывает профиль текущего узла.
	/// </summary>
	public sealed class NodeProfile
		: _Profile_Proto
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NodeProfile"/> со значениями по умолчанию.
		/// </summary>
		public NodeProfile()
		{
			Type = NodeProfileType.Node;
			ChildNodes = new List<NodeProfile>();
			Pages = new List<PageProfile>();
		}


		/* properties */


		/// <summary>
		/// Получает или задает уникальное имя, идентифицирующее информационный узел в рамках карты сайта.
		/// </summary>
		/// <value>Строковый идентификатор узла или <see langword="null"/> для виртуальных/ссылочных узлов.</value>
		public string? Name { get; set; }


		/// <summary>
		/// Получает или задает вышестоящий (родительский) информационный узел в структуре сайта.
		/// Если узел находится на самом верхнем (корневом) уровне, возвращает <see langword="null"/>.
		/// </summary>
		public NodeProfile? ParentNode { get; set; }


		/// <summary>
		/// Получает или задает коллекцию прямых подчиненных (дочерних) информационных узлов.
		/// </summary>
		/// <value>Коллекция дочерних объектов <see cref="NodeProfile"/>. Гарантированно не возвращает <see langword="null"/>.</value>
		public ICollection<NodeProfile> ChildNodes { get; set; }


		/// <summary>
		/// Получает или задает коллекцию корневых страниц, принадлежащих данному узлу.
		/// </summary>
		/// <value>Коллекция объектов <see cref="PageProfile"/>, привязанных к узлу. Гарантированно не возвращает <see langword="null"/>.</value>
		public ICollection<PageProfile> Pages { get; set; }


		/* readonly properties */


		/// <summary>
		/// Получает семантический тип информационного узла, определяющий его поведение в навигации.
		/// </summary>
		public NodeProfileType Type { get; }


		/* methods */


		public void Init(
			NodemapItem source)
		{
			Name = source.Name;
			SetFace(source.Face);
			if (source.ParentNode is not null)
			{
				ParentNode = new NodeProfile();
				ParentNode.Init(source.ParentNode);
			}
		}


		/* overrides */


		public override IEnumerable<LinkBuilder>? GetParentsLinks()
		{
			if (Name is null)
				return null;
			var items1 = new List<LinkBuilder>();
			var parents1 = ParentNode?.GetParentsLinks();
			if (parents1 is not null)
				items1.AddRange();
			items1.Add(new LinkBuilder($"/{Name}", ShortTitle));
			return items1;
		}

	}

}
