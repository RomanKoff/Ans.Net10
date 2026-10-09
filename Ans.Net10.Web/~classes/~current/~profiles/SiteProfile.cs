namespace Ans.Net10.Web
{

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


		public string? HomeUrl { get; set; } = null;


		/// <summary>
		/// Получает или задает коллекцию корневых информационных узлов первого уровня, принадлежащих сайту.
		/// </summary>
		/// <value>Список объектов <see cref="NodeProfile"/>, у которых отсутствует родительский узел.</value>
		public ICollection<NodeProfile> RootNodes { get; set; }

	}

}
