// rev 2026-10-09

using Ans.Net10.Common;
using Ans.Net10.Common.Services;
using Ans.Net10.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Единый оркестровый контекст текущего HTTP-запроса (Context Hub), инкапсулирующий 
	/// базовые инфраструктурные службы платформы и встроенные прикладные сервисы библиотеки.
	/// </summary>
	public partial class CurrentContext
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="CurrentContext"/> с внедрением всех базовых зависимостей.
		/// </summary>
		/// <param name="env">Среда выполнения веб-приложения.</param>
		/// <param name="configuration">Конфигурация приложения.</param>
		/// <param name="httpContextAccessor">Служба доступа к текущему контексту HTTP-запроса.</param>
		/// <param name="hybridCache">Служба двухслойного гибридного кэширования платформы.</param>
		/// <param name="viewRender">Служба изолированного рендеринга представлений Razor в строковые буферы.</param>
		/// <param name="httpClientFactory">Фабрика для создания экземпляров HTTP-клиентов.</param>
		/// <param name="mailer">Служба асинхронной отправки электронных писем.</param>
		/// <param name="linkGenerator">Компонент генерации URL-адресов на основе зарегистрированных маршрутов приложения.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если любой из входящих инфраструктурных сервисов равен <see langword="null"/>.
		/// </exception>
		public CurrentContext(
			ILoggerFactory loggerFactory,
			IWebHostEnvironment env,
			IConfiguration configuration,
			IHttpContextAccessor httpContextAccessor,
			HybridCache hybridCache,
			IViewRenderService viewRender,
			IHttpClientFactory httpClientFactory,
			IMailerService mailer,
			LinkGenerator linkGenerator)
		{
			ArgumentNullException.ThrowIfNull(env);
			ArgumentNullException.ThrowIfNull(configuration);
			ArgumentNullException.ThrowIfNull(httpContextAccessor);
			ArgumentNullException.ThrowIfNull(hybridCache);
			ArgumentNullException.ThrowIfNull(viewRender);
			ArgumentNullException.ThrowIfNull(httpClientFactory);
			ArgumentNullException.ThrowIfNull(mailer);
			ArgumentNullException.ThrowIfNull(linkGenerator);

			Logger = loggerFactory.CreateLogger("Ans.Net10.Web");
			Env = env;
			Configuration = configuration;
			HttpContext = httpContextAccessor.HttpContext
				?? throw new Exception("HttpContext is busy!");
			HybridCache = hybridCache;
			ViewRender = viewRender;
			HttpClientFactory = httpClientFactory;
			Mailer = mailer;
			LinkGenerator = linkGenerator;

			Options = AppSettingsFactory.GetOptions<LibWebOptions>(Configuration);
			Culture = CultureInfo.CurrentCulture;
			DateTimeHelper = new();

			// datas
			Host = new(this);

			// services
			Cache = new(this);
			Cookies = new(this);
			Network = new(this);
			QueryString = new(this);
			Send = new(this);
			WebApi = new(this);

			// profiles
			Site = new();
			Node = new();
			Page = new();
		}


		/* readonly properties */


		/// <summary>
		/// Централизованная служба логирования Ans-экосистемы.
		/// </summary>
		public ILogger Logger { get; }


		/// <summary>
		/// Получает объект среды выполнения веб-приложения (Development, Staging, Production).
		/// </summary>
		public IWebHostEnvironment Env { get; }


		/// <summary>
		/// Получает конфигурацию текущего веб-приложения.
		/// </summary>
		public IConfiguration Configuration { get; }


		/// <summary>
		/// Получает контекст текущего HTTP-запроса.
		/// </summary>
		/// <value>
		/// Объект <see cref="HttpContext"/> для текущего запроса, или <see langword="null"/>, 
		/// если контекст недоступен (например, при вызове вне контекста веб-сервера).
		/// </value>
		public HttpContext HttpContext { get; }


		/// <summary>
		/// Получает службу гибридного кэширования данных <see cref="Microsoft.Extensions.Caching.Hybrid.HybridCache"/>.
		/// </summary>
		public HybridCache HybridCache { get; }


		/// <summary>
		/// Получает службу рендеринга Razor-представлений в строки.
		/// </summary>
		public IViewRenderService ViewRender { get; }


		/// <summary>
		/// Получает фабрику для создания именованных и типизированных HTTP-клиентов.
		/// </summary>
		public IHttpClientFactory HttpClientFactory { get; }


		/// <summary>
		/// Получает службу отправки электронной почты SMTP.
		/// </summary>
		public IMailerService Mailer { get; }


		/// <summary>
		/// Получает генератор URL-адресов на основе зарегистрированной системы маршрутов.
		/// </summary>
		public LinkGenerator LinkGenerator { get; }


		/// <summary>
		/// Получает строго типизированные параметры конфигурации веб-библиотеки.
		/// </summary>
		public LibWebOptions Options { get; }


		/// <summary>
		/// Получает информацию о текущей культуре и языковых стандартах потока.
		/// </summary>
		public CultureInfo Culture { get; }


		/// <summary>
		/// Получает экземпляр помощника для работы с форматированием дат и времени.
		/// </summary>
		public DateTimeHelper DateTimeHelper { get; }


		/* datas */


		public HostData Host { get; }


		/* services */


		/// <summary>
		/// Получает прикладную службу гибридного кэширования текущего контекста.
		/// </summary>
		public CacheService Cache { get; }


		/// <summary>
		/// Получает службу безопасного чтения и записи файлов Cookie.
		/// </summary>
		public CookiesService Cookies { get; }


		/// <summary>
		/// Получает службу сетевого анализа подключения и валидации IP-адресов/подсетей.
		/// </summary>
		public NetworkService Network { get; }


		/// <summary>
		/// Получает службу для работы со строкой запроса (Query String) и фильтрации URL параметров.
		/// </summary>
		public QueryStringService QueryString { get; }


		/// <summary>
		/// Получает службу отправки уведомлений и HTML-писем на базе Razor-представлений.
		/// </summary>
		public SendService Send { get; }


		/// <summary>
		/// Получает службу выполнения запросов к внешним Web API с поддержкой гибридного кэширования.
		/// </summary>
		public WebApiService WebApi { get; }


		/* profiles */


		/// <summary>
		/// Профиль сайта.
		/// </summary>
		public SiteProfile Site { get; }


		/// <summary>
		/// Профиль узла.
		/// </summary>
		public NodeProfile Node { get; }


		/// <summary>
		/// Профиль страницы.
		/// </summary>
		public PageProfile Page { get; }


		/* functions */


		public string GetAbsoluteUrl(
			string? target)
		{
			if (string.IsNullOrWhiteSpace(target))
				return string.Empty;
			if (target.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
				target.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
				return target;
			return $"{Host.VirtualPath}/{target.TrimStart('/')}";
		}


		/// <summary>
		/// Автоматически собранный человекочитаемый заголовок веб-страницы для тега &lt;title&gt; браузера.
		/// Объединяет иерархию узлов и название текущей страницы с использованием интерполяции .NET 10.
		/// </summary>
		/// <remarks>
		/// Формирует строку вида: "Название страницы | Кафедра -> Факультет -> Название сайта".
		/// Если страница или узел не определены, возвращает базовое название сайта или пустую строку.
		/// </remarks>
		public string BrowserTitle
			=> field ??= _getCalculatedBrowserTitle();


		/// <summary>
		/// Получает безопасное HTML-представление человекочитаемый заголовок веб-страницы для тега &lt;title&gt; браузера.
		/// </summary>		
		public HtmlString BrowserTitleHtml
			=> field ??= BrowserTitle.ToHtml(false);


		/// <summary>
		/// Возвращает ленивое перечисление элементов навигационной цепочки ("хлебных крошек") 
		/// от текущей страницы/узла вверх до корня сайта.
		/// </summary>
		public IEnumerable<string> Breadcrumbs
			=> _getCalculatedBreadcrumbs();


		/* todo */


		internal string GetResUrl(string v)
		{
			throw new NotImplementedException();
		}


		internal string GetUrl(string v)
		{
			throw new NotImplementedException();
		}


		/* privates */


		private string _getCalculatedBrowserTitle()
		{
			return string.Empty;
		}


		private IEnumerable<string> _getCalculatedBreadcrumbs()
		{
			return [];
		}

	}

}
