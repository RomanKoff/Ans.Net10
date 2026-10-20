// rev 2026-10-20

using Ans.Net10.Common;
using Ans.Net10.Common.Services;
using Ans.Net10.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;

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
		/// <param name="loggerFactory">Фабрика для создания системных регистраторов логов.</param>
		/// <param name="env">Среда выполнения веб-приложения.</param>
		/// <param name="configuration">Конфигурация приложения.</param>
		/// <param name="httpContextAccessor">Служба доступа к текущему контексту HTTP-запроса.</param>
		/// <param name="hybridCache">Служба двухслойного гибридного кэширования платформы.</param>
		/// <param name="viewRender">Служба изолированного рендеринга представлений Razor в строковые буферы.</param>
		/// <param name="httpClientFactory">Фабрика для создания экземпляров HTTP-клиентов.</param>		
		/// <param name="sitemap">Поставщик данных и управления жизненным циклом карты сайта.</param>
		/// <param name="mailer">Служба асинхронной отправки электронных писем.</param>
		/// <param name="linkGenerator">Компонент генерации URL-адресов на основе зарегистрированных маршрутов приложения.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если любой из входящих инфраструктурных сервисов равен <see langword="null"/>.
		/// </exception>
		/// <exception cref="Exception">
		/// Вызывается при критическом сбое системы, если текущий контекст <see cref="HttpContext"/> недоступен через аксессор.
		/// </exception>
		public CurrentContext(
			ILoggerFactory loggerFactory,
			IWebHostEnvironment env,
			IConfiguration configuration,
			IHttpContextAccessor httpContextAccessor,
			HybridCache hybridCache,
			IViewRenderService viewRender,
			IHttpClientFactory httpClientFactory,
			ISitemapProvider sitemap,
			IMailerService mailer,
			LinkGenerator linkGenerator)
		{
			ArgumentNullException.ThrowIfNull(loggerFactory);
			ArgumentNullException.ThrowIfNull(env);
			ArgumentNullException.ThrowIfNull(configuration);
			ArgumentNullException.ThrowIfNull(httpContextAccessor);
			ArgumentNullException.ThrowIfNull(hybridCache);
			ArgumentNullException.ThrowIfNull(viewRender);
			ArgumentNullException.ThrowIfNull(httpClientFactory);
			ArgumentNullException.ThrowIfNull(mailer);
			ArgumentNullException.ThrowIfNull(linkGenerator);

			// base
			Logger = loggerFactory.CreateLogger("Ans.Net10.Web");
			Env = env;
			Configuration = configuration;
			HttpContext = httpContextAccessor.HttpContext
				?? throw new Exception("HttpContext is busy!");
			HybridCache = hybridCache;
			ViewRender = viewRender;
			HttpClientFactory = httpClientFactory;
			Sitemap = sitemap;
			Mailer = mailer;
			LinkGenerator = linkGenerator;

			Options = AppSettingsFactory.GetOptions<LibWebOptions>(Configuration);
			Culture = CultureInfo.CurrentCulture;
			DateTimeHelper = new();

			// datas
			Host = new(this);
			Request = new(this);

			// services
			Cache = new(this);
			Cookies = new(this);
			Network = new(this);
			QueryString = new(this);
			Send = new(this);
			WebApi = new(this);

			// profiles
			Site = new(this);
			Node = new(this);
			Page = new(this);
		}


		/* base  */


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
		/// Получает гарантированно доступный контекст текущего HTTP-запроса.
		/// </summary>
		public HttpContext HttpContext { get; }


		/// <summary>
		/// Получает службу гибридного кэширования данных <see cref="HybridCache"/>.
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


		///// <summary>
		///// Получает поставщик данных и управления жизненным циклом карты сайта.
		///// </summary>
		public ISitemapProvider Sitemap { get; }


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


		/// <summary>
		/// Получает структурированные данные о сетевом адресе хоста и виртуальных путях приложения.
		/// </summary>
		public HostData Host { get; }


		/// <summary>
		/// Получает структурированные данные о параметрах роутинга текущего входящего HTTP-запроса,
		/// включая информацию об областях (Areas), контроллерах, экшенах и страницах Razor.
		/// </summary>
		public RequestData Request { get; }


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
		/// Профиль текущего сайта.
		/// </summary>
		public SiteProfile Site { get; }


		/// <summary>
		/// Профиль текущего информационного узла.
		/// </summary>
		public NodeProfile Node { get; }


		/// <summary>
		/// Профиль текущей контентной страницы.
		/// </summary>
		public PageProfile Page { get; }


		/* readonly properties */


		/// <summary>
		/// Получает признак того, что веб-приложение запущено в среде разработки (Development).
		/// </summary>
		/// <value>Значение <see langword="true"/>, если среда выполнения соответствует <see cref="Microsoft.Extensions.Hosting.Environments.Development"/>; иначе — <see langword="false"/>.</value>
		public bool IsDevelopment
			=> Env.IsDevelopment();


		/// <summary>
		/// Получает безопасное HTML-представление человекочитаемого заголовка веб-страницы для тега &lt;title&gt; браузера.
		/// </summary>		
		/// <value>Объект <see cref="HtmlString"/>, готовый к выводу в мастер-шаблоне без повторного экранирования.</value>
		public HtmlString BrowserTitleHtml
			=> field ??= new HtmlString(GetBrowserTitle());


		/// <summary>
		/// Возвращает последовательность элементов навигационной цепочки («хлебных крошек») 
		/// от главной страницы сайта до текущего активного элемента контента.
		/// </summary>
		/// <value>
		/// Коллекция объектов <see cref="LinkBuilder"/>, представляющих элементы навигации, 
		/// или <see langword="null"/>, если сайт не инициализирован.
		/// </value>
		public IEnumerable<LinkBuilder>? Breadcrumbs
			=> field ??= _getBreadcrumbsLazy();


		/* properties */


		/// <summary>
		/// Получает или задает имя основного (базового) файла разметки интерфейса сайта по умолчанию.
		/// </summary>
		/// <value>Строка с путем или именем макета Razor (например, <c>"_Layout"</c>), инициализируемая из глобальных настроек библиотеки.</value>
		public string DefaultLayout
		{
			get => field ??= Options.DefaultLayout;
			set;
		}


		/// <summary>
		/// Получает или задает имя файла разметки для системных и административных страниц интерфейса.
		/// </summary>
		/// <value>Строка с именем системного макета Razor. Если не задана явно, возвращает значение <see cref="DefaultLayout"/>.</value>
		public string SystemLayout
		{
			get => field ??= (Options.SystemLayout ?? DefaultLayout);
			set;
		}


		/// <summary>
		/// Получает или задает имя файла разметки для страниц отображения HTTP-ошибок (404, 403, 500 и т.д.).
		/// </summary>
		/// <value>Строка с именем макета Razor для вывода ошибок. Если не задана явно, возвращает значение <see cref="DefaultLayout"/>.</value>
		public string ErrorsLayout
		{
			get => field ??= (Options.Errors.Layout ?? DefaultLayout);
			set;
		}


		/* functions */


		/// <summary>
		/// Формирует абсолютный URL-адрес для указанного относительного пути с учетом виртуального каталога хоста.
		/// </summary>
		/// <param name="target">Целевой относительный путь или URL.</param>
		/// <returns>Полный абсолютный путь, готовый для использования в ссылках.</returns>
		public string GetAbsoluteUrl(
			string? target)
		{
			if (string.IsNullOrWhiteSpace(target))
				return string.Empty;
			if (target.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
				|| target.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
				return target;
			var baseVirtual1 = Host.VirtualPath;
			var cleanTarget1 = target.TrimStart('/');
			return baseVirtual1 is { Length: > 0 }
				? $"{baseVirtual1}/{cleanTarget1}"
				: $"/{cleanTarget1}";
		}


		/// <summary>
		/// Формирует собранный человекочитаемый заголовок веб-страницы для тега &lt;title&gt; браузера.
		/// Объединяет иерархию сокращенных названий страницы, узла и сайта.
		/// </summary>
		/// <value>Строка готового заголовка окна браузера.</value>
		public string GetBrowserTitle()
		{
			if (!string.IsNullOrWhiteSpace(Page.CustomBrowserTitle))
				return Page.CustomBrowserTitle;
			return $"{Page.ShortTitle.Make("{0} – ")}{Node.ShortTitle.Make("{0} – ")}{Site.ShortTitle}";
		}


		/// <summary>
		/// Формирует полный URL-адрес статического или медиа-ресурса CMS на основе переданной семантической строки назначения.
		/// </summary>
		/// <param name="target">Строка назначения ресурса или прямой URL.</param>
		/// <returns>Полный URL-адрес для вывода в тегах разметки (например, изображений, скриптов, стилей).</returns>
		public string GetResUrl(
			string? target)
		{
			if (string.IsNullOrEmpty(target))
				return string.Empty;
			//if (target[0] == '/')
			//	return $"{Site.ResUrl}{target}";
			//if (target.StartsWith("site:"))
			//	return $"{Site.ResUrl}{target[5..].Make("/{0}")}";
			//if (target.StartsWith("node:"))
			//	return $"{Node.ResUrl}{target[5..].Make("/{0}")}";
			//if (target.StartsWith("page:"))
			//	return $"{Page.ResUrl}{target[5..].Make("/{0}")}";
			return target;
		}


		/// <summary>
		/// Разрешает и формирует относительный или абсолютный URL-адрес внутренней ЧПУ-страницы на основе переданного маршрутного плейсхолдера.
		/// </summary>
		/// <param name="target">Строка навигационного назначения или прямой URL-путь.</param>
		/// <returns>Готовая строка URL-адреса для использования в гиперссылках веб-интерфейса.</returns>
		public string GetUrl(
			string? target)
		{
			if (string.IsNullOrEmpty(target))
				return string.Empty;
			//if (target[0] == '/')
			//	return $"{Site.AppUrl}{target}";
			//if (target.StartsWith("site:"))
			//	return $"{Site.AppUrl}{target[5..].Make("/{0}")}";
			//if (target.StartsWith("node:"))
			//	return $"{Node.AppUrl}{target[5..].Make("/{0}")}";
			//if (target.StartsWith("page:"))
			//	return $"{Page.AppUrl}{target[5..].Make("/{0}")}";
			return target;
		}


		/* methods */


		/// <summary>
		/// Генерирует и выбрасывает специализированное HTTP-исключение со статусом <c>404 Not Found</c> (Ресурс не найден).
		/// </summary>
		/// <remarks>
		/// Данный метод прерывает стандартный цикл выполнения текущего HTTP-запроса и делегирует управление глобальному обработчику ошибок.
		/// </remarks>
		/// <exception cref="AnsHttpException">Вызывается всегда при выполнении метода со статусом <see cref="HttpStatusCode.NotFound"/>.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
		public void ThrowNotFound()
		{
			throw new AnsHttpException(HttpStatusCode.NotFound);
		}


		/// <summary>
		/// Генерирует и выбрасывает специализированное HTTP-исключение со статусом <c>403 Forbidden</c> (Доступ запрещен).
		/// </summary>
		/// <remarks>
		/// Используется в прикладных сценариях авторизации контента для немедленной блокировки несанкционированного доступа.
		/// </remarks>
		/// <exception cref="AnsHttpException">Вызывается всегда при выполнении метода со статусом <see cref="HttpStatusCode.Forbidden"/>.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
		public void ThrowForbidden()
		{
			throw new AnsHttpException(HttpStatusCode.Forbidden);
		}


		/* privates */


		private static List<LinkBuilder> _getBreadcrumbsLazy()
		{
			var items1 = new List<LinkBuilder>();
			return items1;
		}

	}

}
