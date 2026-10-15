// rev 2026-10-14

namespace Ans.Net10.Web
{

	/// <summary>
	/// Перечисление семантических типов профилей страниц.
	/// </summary>
	public enum PageProfileType
	{
		/// <summary>
		/// Стандартная страница контента (может быть как изолированной, так и иметь дочерние страницы).
		/// </summary>
		Normal,

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
	/// Описывает профиль текущей страницы.
	/// </summary>
	public sealed class PageProfile
		: _Profile_Proto
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="PageProfile"/> со значениями по умолчанию.
		/// </summary>
		public PageProfile()
		{
			Type = PageProfileType.Normal;
			ChildPages = new List<PageProfile>();
		}


		/* properties */


		/// <summary>
		/// Получает или задает вышестоящую (родительскую) страницу в иерархическом дереве контента узла.
		/// Если страница находится на корневом уровне информационного узла, возвращает <see langword="null"/>.
		/// </summary>
		public PageProfile? ParentPage { get; set; }


		/// <summary>
		/// Получает или задает коллекцию прямых подчиненных (дочерних) контентных страниц.
		/// </summary>
		/// <value>Коллекция дочерних объектов <see cref="PageProfile"/>. Гарантированно не возвращает <see langword="null"/>.</value>
		public ICollection<PageProfile>? ChildPages { get; set; }


		/// <summary>
		/// Получает или задает имя страницы, уникально идентифицирующее её в пределах текущей ветки контента.
		/// </summary>
		/// <value>Строковый идентификатор страницы или <see langword="null"/> для виртуальных/ссылочных страниц.</value>
		public string? Name { get; set; }


		/// <summary>
		/// Получает или задает семантический тип страницы, определяющий характер перехода в интерфейсе.
		/// </summary>
		public PageProfileType Type { get; set; }


		/// <summary>
		/// Получает или задает настраиваемый (перегруженный) заголовок страницы для браузера.
		/// Если свойство заполнено, оно имеет наивысший приоритет при формировании заголовка окна.
		/// </summary>
		/// <value>Строка кастомного заголовка или <see langword="null"/>, если заголовок должен строиться по умолчанию.</value>
		public string? CustomBrowserTitle { get; set; }



		/* readonly properties */


		/// <summary>
		/// Получает полный относительный ЧПУ-путь страницы контента, сформированный с учетом родительских страниц-предков.
		/// </summary>
		/// <value>Строка ЧПУ-маршрута страницы или <see langword="null"/>, если это виртуальная ссылочная страница.</value>
		public string? Path { get; internal set; }

	}

}
