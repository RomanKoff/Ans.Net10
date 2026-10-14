// rev 2026-10-14

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


		/// <summary>
		/// Получает или задает базовый абсолютный или относительный URL-адрес главной страницы сайта.
		/// </summary>
		/// <value>Строка адреса главной страницы. По умолчанию инициализируется пустой строкой.</value>
		public string HomeUrl { get; set; } = string.Empty;

	}

}
