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
		/// <remarks>
		/// В процессе инициализации формируется абсолютный URL-адрес корня веб-приложения <see cref="ApplicationUrl"/> 
		/// путем конкатенации базового доменного адреса <see cref="BaseUrl"/> и виртуального префикса <see cref="VirtualPath"/>.
		/// </remarks>
		/// <param name="current">Единый оркестровый контекст текущего HTTP-запроса.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="current"/> равен <see langword="null"/> 
		/// или связанный с ним контекст <see cref="Microsoft.AspNetCore.Http.HttpContext"/> не был инициализирован.
		/// </exception>
		public HostData(
			CurrentContext current)
		{
			ArgumentNullException.ThrowIfNull(current);
			ArgumentNullException.ThrowIfNull(current.HttpContext);
			BaseUrl = current.HttpContext.GetBaseUrl();
			VirtualPath = current.HttpContext.GetVirtualPath();
			ApplicationUrl = current.HttpContext.GetApplicationUrl();
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает базовый URL-адрес веб-приложения (протокол, хост и порт, если он нестандартный) 
		/// </summary>
		/// <value>Строка вида <c>https://mysite.com</c> или <c>http://localhost:5173</c>.</value>
		public string BaseUrl { get; }


		/// <summary>
		/// Возвращает базовый виртуальный путь (PathBase) веб-приложения, если оно развернуто в подкаталоге IIS/Nginx.
		/// </summary>
		/// <returns>Строка виртуального пути вида <c>"/app_name"</c>, или <see langword="null"/>, если приложение запущено в корне.</returns>
		public string? VirtualPath { get; }


		/// <summary>
		/// Возвращает полный абсолютный URL-адрес корня веб-приложения, включая виртуальный путь подкаталога.
		/// </summary>
		/// <returns>
		/// Строка вида
		/// <c>"https://mysite.com"</c>,
		/// <c>"https://mysite.com/app_name"</c>,
		/// <c>"http://localhost:5173"</c> или
		/// <c>"http://localhost:5173/app_name"</c>.</returns>
		public string ApplicationUrl { get; }

	}

}
