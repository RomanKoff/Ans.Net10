// rev 2026-10-21

using System.Xml.Serialization;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Объект переноса данных (DTO) для корневого элемента конфигурации дерева информационных узлов.
	/// </summary>
	[XmlRoot("nodes", Namespace = "urn:ans:net10:web:map-nodes")]
	public sealed class NodesXmlDto
	{
		/// <summary>
		/// Получает или задает список корневых элементов навигационных узлов.
		/// </summary>
		/// <returns>Список объектов <see cref="NodeItemXmlDto"/> или <see langword="null"/>, если элементы не заданы.</returns>
		[XmlElement("item")]
		public List<NodeItemXmlDto>? Items { get; set; }
	}




	/// <summary>
	/// Объект переноса данных (DTO) для конкретного элемента информационного узла в XML-файле.
	/// </summary>
	public sealed class NodeItemXmlDto
	{
		/// <summary>
		/// Получает или задает список дочерних элементов информационных узлов.
		/// </summary>
		/// <returns>Список дочерних объектов <see cref="NodeItemXmlDto"/> или <see langword="null"/>, если вложенные элементы отсутствуют.</returns>
		[XmlElement("item")]
		public List<NodeItemXmlDto>? ChildItems { get; set; }

		/// <summary>
		/// Получает или задает строку назначения (идентификатор узла, внутренний или внешний URL).
		/// </summary>
		/// <returns>Строка назначения (ЧПУ-идентификатор, относительный путь или внешний адрес) либо <see langword="null"/>.</returns>
		[XmlAttribute("target")]
		public string? Target { get; set; }

		/// <summary>
		/// Получает или задает человекочитаемое наименование (интерфейс) или шаблон названия элемента.
		/// </summary>
		/// <returns>Человекочитаемый заголовок или маска отображения узла.</returns>
		[XmlAttribute("face")]
		public string Face { get; set; } = string.Empty;
	}



	/// <summary>
	/// Объект переноса данных (DTO) для корневого элемента конфигурации дерева страниц конкретного узла.
	/// </summary>
	[XmlRoot("pages", Namespace = "urn:ans:net10:web:map-pages")]
	public sealed class PagesXmlDto
	{
		/// <summary>
		/// Получает или задает список корневых элементов контентных страниц.
		/// </summary>
		/// <returns>Список объектов <see cref="PageItemXmlDto"/> или <see langword="null"/>, если страницы не заданы.</returns>
		[XmlElement("item")]
		public List<PageItemXmlDto>? Items { get; set; }
	}



	/// <summary>
	/// Объект переноса данных (DTO) для конкретного элемента контентной страницы в XML-файле.
	/// </summary>
	public sealed class PageItemXmlDto
	{
		/// <summary>
		/// Получает или задает список дочерних элементов контентных страниц.
		/// </summary>
		/// <returns>Список дочерних объектов <see cref="PageItemXmlDto"/> или <see langword="null"/>, если вложенные страницы отсутствуют.</returns>
		[XmlElement("item")]
		public List<PageItemXmlDto>? ChildItems { get; set; }

		/// <summary>
		/// Получает или задает строку назначения страницы (ЧПУ-имя, внутренний или внешний URL).
		/// </summary>
		/// <returns>Строка ЧПУ-имени страницы или адрес перенаправления.</returns>
		[XmlAttribute("target")]
		public string Target { get; set; } = string.Empty;

		/// <summary>
		/// Получает или задает человекочитаемое наименование страницы.
		/// </summary>
		/// <returns>Текст названия страницы для отображения в интерфейсах либо <see langword="null"/>.</returns>
		[XmlAttribute("face")]
		public string? Face { get; set; }

		/// <summary>
		/// Получает или задает флаг, указывающий на отсутствие у данного элемента стартовой страницы.
		/// </summary>
		/// <returns><see langword="true"/>, если для данного уровня структуры отключена индексная страница; иначе — <see langword="false"/>.</returns>
		[XmlAttribute("no-index")]
		public bool HasNoIndex { get; set; }
	}

}
