// rev 2026-10-10

namespace Ans.Net10.Web
{

	/// <summary>
	/// Описывает профиль текущего сайта.
	/// </summary>
	public sealed class SiteProfile
		: _Profile_Proto
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="SiteProfile"/> со значениями по умолчанию.
		/// </summary>
		public SiteProfile()
		{
			RootNodes = new List<NodeProfile>();
		}


		/* properties */


		/// <summary>
		/// Получает или задает коллекцию корневых информационных узлов первого уровня, принадлежащих сайту.
		/// </summary>
		/// <value>Список объектов <see cref="NodeProfile"/>, у которых отсутствует родительский узел. Гарантированно не возвращает <see langword="null"/>.</value>
		public ICollection<NodeProfile> RootNodes { get; set; }


		/* methods */


		/// <summary>
		/// Выполняет расчет иерархической навигации, хлебных крошек и заголовков браузера 
		/// для текущего профиля сайта.
		/// </summary>		
		public override void CalcPartNavigation()
		{
			throw new NotImplementedException();
		}

	}

}
