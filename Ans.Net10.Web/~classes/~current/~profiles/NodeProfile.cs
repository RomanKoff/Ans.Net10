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



	public sealed class NodeProfile
		: _Profile_Proto
	{

		/* ctor */


		public NodeProfile()
		{
			Type = NodeProfileType.Normal;
			ChildNodes = new List<NodeProfile>();
			Pages = new List<PageProfile>();
		}


		/* properties */


		public string? Name { get; set; } = null;


		/// <summary>
		/// Получает или задает семантический тип информационного узла, определяющий его поведение в навигации.
		/// </summary>
		public NodeProfileType Type { get; set; }


		/// <summary>
		/// Получает или задает вышестоящий (родительский) информационный узел в структуре сайта.
		/// </summary>
		public NodeProfile? ParentNode { get; set; }


		/// <summary>
		/// Получает или задает коллекцию прямых подчиненных (дочерних) информационных узлов.
		/// </summary>
		public ICollection<NodeProfile> ChildNodes { get; set; }


		/// <summary>
		/// Получает или задает коллекцию корневых страниц, принадлежащих данному узлу.
		/// </summary>
		public ICollection<PageProfile> Pages { get; set; }

	}

}
