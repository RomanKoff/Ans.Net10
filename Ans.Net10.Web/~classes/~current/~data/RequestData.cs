using Microsoft.AspNetCore.Routing;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Предоставляет структурированные данные о параметрах роутинга текущего входящего HTTP-запроса,
	/// включая информацию об областях (Areas), контроллерах, экшенах и страницах Razor.
	/// </summary>
	public sealed class RequestData
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="RequestData"/> на основе текущего контекста приложения.
		/// </summary>
		/// <param name="current">Единый оркестровый контекст текущего HTTP-запроса.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="current"/> равен <see langword="null"/>.</exception>
		public RequestData(
			CurrentContext current)
		{
			ArgumentNullException.ThrowIfNull(current);
			var request1 = current.HttpContext.Request;
			RelativeUrl = request1.Path.ToString().ToLowerInvariant().TrimEnd('/');
			AbsoluteUrl = $"{current.Host.ApplicationUrl}{RelativeUrl}";
			Params = request1.QueryString.Value;
			AreaName = current.HttpContext.GetRouteValue("Area")?.ToString();
			RazorPage = current.HttpContext.GetRouteValue("Page")?.ToString();
			ControllerName = current.HttpContext.GetRouteValue("Controller")?.ToString();
			ActionName = current.HttpContext.GetRouteValue("Action")?.ToString();
			IsController = !string.IsNullOrEmpty(ControllerName);
			IsRazorPage = !string.IsNullOrEmpty(RazorPage);
			ActionUniqueID = _buildActionUniqueId();
		}


		/* readonly properties */


		/// <summary>
		/// Получает относительный URL-адрес текущего запроса в нижнем регистре без закрывающего слэша.
		/// </summary>
		public string RelativeUrl { get; }


		/// <summary>
		/// Получает полный абсолютный URL-адрес текущего запроса.
		/// </summary>
		public string AbsoluteUrl { get; }


		/// <summary>
		/// Получает сырую строку параметров запроса (Query String) вместе со знаком "?".
		/// </summary>
		/// <value>Строка параметров запроса или <see langword="null"/>, если параметры отсутствуют.</value>
		public string? Params { get; }


		/// <summary>
		/// Получает имя текущей области (Area) маршрутизации, если она задана.
		/// </summary>
		/// <value>Имя области или <see langword="null"/>, если область не определена в маршруте.</value>
		public string? AreaName { get; }


		/// <summary>
		/// Получает имя текущего MVC-контроллера, если запрос обрабатывается контроллером.
		/// </summary>
		/// <value>Имя контроллера или <see langword="null"/>, если запрос идет к Razor Page или статическому файлу.</value>
		public string? ControllerName { get; }


		/// <summary>
		/// Получает имя текущего действия (Action) контроллера.
		/// </summary>
		/// <value>Имя действия или <see langword="null"/>, если запрос обрабатывается подсистемой Razor Pages.</value>
		public string? ActionName { get; }


		/// <summary>
		/// Получает путь к текущей странице Razor, если запрос обрабатывается Razor Pages.
		/// </summary>
		/// <value>Виртуальный путь к странице или <see langword="null"/>, если запрос обрабатывается классическим контроллером.</value>
		public string? RazorPage { get; }


		/// <summary>
		/// Получает уникальный строковый идентификатор выполненного действия в формате ЧПУ нижнего регистра.
		/// </summary>
		public string ActionUniqueID { get; }


		/// <summary>
		/// Получает признак того, что текущий запрос обрабатывается классическим MVC-контроллером.
		/// </summary>
		public bool IsController { get; }


		/// <summary>
		/// Получает признак того, что текущий запрос обрабатывается подсистемой Razor Pages.
		/// </summary>
		public bool IsRazorPage { get; }


		/* privates */


		private string _buildActionUniqueId()
		{
			string areaPart1 = string.IsNullOrEmpty(AreaName)
				? string.Empty
				: $"/{AreaName}";
			string pagePart1 = string.IsNullOrEmpty(RazorPage)
				? string.Empty
				: RazorPage;
			string controllerPart1 = string.IsNullOrEmpty(ControllerName)
				? string.Empty
				: $"/{ControllerName}";
			string actionPart1 = string.IsNullOrEmpty(ActionName)
				? string.Empty
				: $"/{ActionName}";
			return $"{areaPart1}{pagePart1}{controllerPart1}{actionPart1}".ToLowerInvariant();
		}

	}

}
