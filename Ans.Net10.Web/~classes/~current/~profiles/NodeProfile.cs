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
		Normal,

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
			Type = NodeProfileType.Normal;
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
		/// Получает или задает семантический тип информационного узла, определяющий его поведение в навигации.
		/// </summary>
		public NodeProfileType Type { get; set; }


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


		/* methods */


		///// <summary>
		///// Инициализирует профиль узла на основе элемента карты сайта.
		///// </summary>
		///// <param name="item">Элемент карты сайта, содержащий исходные метаданные узла.</param>
		///// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="item"/> равен <see langword="null"/>.</exception>
		//public void Init(
		//	NodemapItem item)
		//{
		//	ArgumentNullException.ThrowIfNull(item);
		//	Name = item.Name;
		//	Type = (NodeProfileType)item.Type;
		//	SetFace(item.Face);
		//}

	}

}
