using Microsoft.AspNetCore.Routing;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Предоставляет структурированные данные о параметрах роутинга текущего входящего HTTP-запроса,
	/// включая информацию об областях (Areas), контроллерах, экшенах и страницах Razor.
	/// </summary>
	public sealed class RequestData
	{

		private readonly CurrentContext _current;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="RequestData"/> на основе текущего контекста приложения.
		/// </summary>
		/// <param name="current">Единый оркестровый контекст текущего HTTP-запроса.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="current"/> равен <see langword="null"/>.</exception>
		public RequestData(
			CurrentContext current)
		{
			_current = current ?? throw new ArgumentNullException(nameof(current));
			var request1 = _current.HttpContext.Request;
			RelativeUrl = request1.Path.ToString().ToLowerInvariant(); // .TrimEnd('/');
			AbsoluteUrl = $"{_current.Host.ApplicationUrl}{RelativeUrl}";
			Params = request1.QueryString.Value;
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
		public string? AreaName
			=> _current.HttpContext.GetRouteValue("Area")?.ToString();


		/// <summary>
		/// Получает имя текущего MVC-контроллера, если запрос обрабатывается контроллером.
		/// </summary>
		public string? ControllerName
			=> _current.HttpContext.GetRouteValue("Controller")?.ToString();


		/// <summary>
		/// Получает имя текущего действия (Action) контроллера.
		/// </summary>
		public string? ActionName
			=> _current.HttpContext.GetRouteValue("Action")?.ToString();


		/// <summary>
		/// Получает путь к текущей странице Razor, если запрос обрабатывается Razor Pages.
		/// </summary>
		public string? RazorPage
			=> _current.HttpContext.GetRouteValue("Page")?.ToString();


		/// <summary>
		/// Получает признак того, что текущий запрос обрабатывается классическим MVC-контроллером.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если задано имя контроллера; иначе — <see langword="false"/>.</value>
		public bool IsController
			=> ControllerName is { Length: > 0 };


		/// <summary>
		/// Получает признак того, что текущий запрос обрабатывается подсистемой Razor Pages.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если определен путь к Razor-странице; иначе — <see langword="false"/>.</value>
		public bool IsRazorPage
			=> RazorPage is { Length: > 0 };


		/// <summary>
		/// Получает уникальный строковый идентификатор выполненного действия в формате ЧПУ нижнего регистра.
		/// </summary>
		public string ActionUniqueID
			=> field ??= _buildActionUniqueId();


		/* privates */


		private string _buildActionUniqueId()
		{
			var area1 = AreaName is { Length: > 0 }
				? $"/{AreaName}"
				: string.Empty;
			var razorPage1 = RazorPage is { Length: > 0 }
				? RazorPage
				: string.Empty;
			var controller1 = ControllerName is { Length: > 0 }
				? $"/{ControllerName}"
				: string.Empty;
			var action1 = ActionName is { Length: > 0 }
				? $"/{ActionName}"
				: string.Empty;
			return $"{area1}{razorPage1}{controller1}{action1}".ToLowerInvariant();
		}

	}

}
