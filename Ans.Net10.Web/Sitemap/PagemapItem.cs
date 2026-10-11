namespace Ans.Net10.Web.Sitemap
{

	/// <summary>
	/// Описывает иммутабельные данные контентной страницы в карте сайта.
	/// </summary>
	public sealed class PagemapItem
	{

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="PagemapItem"/>.
		/// </summary>
		public PagemapItem(
			string target,
			string face,
			bool hasStart)
		{
			Target = target;
			Face = face;
			HasStart = hasStart;
		}

		/// <summary>
		/// Получает сегмент ЧПУ-пути или адрес ссылки.
		/// </summary>
		public string Target { get; }

		/// <summary>
		/// Получает сырую строку автотипографики и заголовков страницы.
		/// </summary>
		public string Face { get; }

		/// <summary>
		/// Получает признак наличия индексной страницы для текущего уровня.
		/// </summary>
		public bool HasStart { get; }

	}

}
