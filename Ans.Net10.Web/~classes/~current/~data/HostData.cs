namespace Ans.Net10.Web
{

	/// <summary>
	/// Предоставляет структурированные данные о сетевом адресе хоста, виртуальном пути 
	/// и полном URL-адресе запущенного веб-приложения на основе текущего контекста запроса.
	/// </summary>
	public class HostData
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="HostData"/> на основе текущего контекста приложения.
		/// </summary>
		/// <param name="current">Единый оркестровый контекст текущего HTTP-запроса.</param>
		public HostData(
			CurrentContext current)
		{
			BaseUrl = current.HttpContext?.GetBaseUrl() ?? string.Empty;
			VirtualPath = current.HttpContext?.GetVirtualPath() ?? string.Empty;
			ApplicationUrl = $"{BaseUrl}{VirtualPath}";
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает базовый URL-адрес веб-приложения (протокол, хост и порт).
		/// Если контекст запроса недоступен, возвращает пустую строку.
		/// </summary>
		/// <value>Строка вида <c>https://mysite.com</c> или <c>http://localhost:5173</c>.</value>
		public string BaseUrl { get; }


		/// <summary>
		/// Возвращает базовый виртуальный путь приложения, если оно развернуто в подкаталоге.
		/// Если контекст запроса недоступен, возвращает пустую строку.
		/// </summary>
		/// <value>Строка пути или пустая строка, если приложение запущено в корне домена.</value>
		public string VirtualPath { get; }


		/// <summary>
		/// Возвращает полный абсолютный URL-адрес корня веб-приложения, 
		/// включая протокол, хост, порт и виртуальный путь подкаталога.
		/// </summary>
		/// <value>Строка вида <c>https://mysite.com</c>.</value>
		public string ApplicationUrl { get; }

	}

}
