// rev 2026-10-10

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
		/// Получает или задает семантический тип страницы, определяющий характер перехода в интерфейсе.
		/// </summary>
		public PageProfileType Type { get; set; }


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


		/* methods */


		/// <summary>
		/// Выполняет расчет иерархической навигации, хлебных крошек и заголовков браузера 
		/// для текущего профиля страницы.
		/// </summary>	
		public override void CalcPartNavigation()
		{
			throw new NotImplementedException();
		}

	}

}
