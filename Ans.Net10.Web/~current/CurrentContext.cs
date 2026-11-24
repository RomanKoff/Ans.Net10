// rev 2026-11-24

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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Единый оркестровый контекст текущего HTTP-запроса (Context Hub), инкапсулирующий 
	/// базовые инфраструктурные службы платформы и встроенные прикладные сервисы библиотеки.
	/// </summary>
	public sealed partial class CurrentContext
	{

		private Dictionary<string, IReadOnlyList<NavItem>>? _breadcrumbsCache;
		private Dictionary<string, IReadOnlyList<NavItem>>? _navCache;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="CurrentContext"/> с внедрением всех базовых зависимостей.
		/// </summary>
		/// <param name="loggerFactory">Фабрика для создания системных регистраторов логов.</param>
		/// <param name="env">Среда выполнения веб-приложения.</param>
		/// <param name="configuration">Конфигурация приложения.</param>
		/// <param name="httpContextAccessor">Служба доступа к текущему контексту HTTP-запроса.</param>
		/// <param name="httpClientFactory">Фабрика для создания экземпляров HTTP-клиентов.</param>		
		/// <param name="hybridCache">Служба двухслойного гибридного кэширования платформы.</param>
		/// <param name="viewRender">Служба изолированного рендеринга представлений Razor в строковые буферы.</param>
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
			IHttpClientFactory httpClientFactory,
			HybridCache hybridCache,
			IViewRenderService viewRender,
			IMailerService mailer,
			LinkGenerator linkGenerator,
			SitemapBuilder sitemapBuilder)
		{
			ArgumentNullException.ThrowIfNull(loggerFactory);
			ArgumentNullException.ThrowIfNull(env);
			ArgumentNullException.ThrowIfNull(configuration);
			ArgumentNullException.ThrowIfNull(httpContextAccessor);
			ArgumentNullException.ThrowIfNull(httpClientFactory);
			ArgumentNullException.ThrowIfNull(hybridCache);
			ArgumentNullException.ThrowIfNull(viewRender);
			ArgumentNullException.ThrowIfNull(mailer);
			ArgumentNullException.ThrowIfNull(linkGenerator);
			ArgumentNullException.ThrowIfNull(sitemapBuilder);

			// base
			Logger = loggerFactory.CreateLogger("Ans.Net10.Web");
			Env = env;
			Configuration = configuration;
			HttpContext = httpContextAccessor.HttpContext
				?? throw new Exception("HttpContext is busy!");
			HttpClientFactory = httpClientFactory;
			HybridCache = hybridCache;
			ViewRender = viewRender;
			Mailer = mailer;
			LinkGenerator = linkGenerator;
			Options = AppSettingsFactory.GetOptions<LibWebOptions>(Configuration);
			Culture = CultureInfo.CurrentCulture;
			DateTimeHelper = new();
			Sitemap = sitemapBuilder;

			// data
			Host = new(this);
			Request = new(this);
			Meta = new(this);

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


		/* base readonly properties */


		/// <summary>
		/// Централизованная служба логирования Ans-экосистемы.
		/// </summary>
		/// <returns>Экземпляр логера <see cref="ILogger"/>.</returns>
		public ILogger Logger { get; }


		/// <summary>
		/// Получает объект среды выполнения веб-приложения (Development, Staging, Production).
		/// </summary>
		/// <returns>Текущая среда выполнения <see cref="IWebHostEnvironment"/>.</returns>
		public IWebHostEnvironment Env { get; }


		/// <summary>
		/// Получает конфигурацию текущего веб-приложения.
		/// </summary>
		/// <returns>Объект конфигурации <see cref="IConfiguration"/>.</returns>
		public IConfiguration Configuration { get; }


		/// <summary>
		/// Получает гарантированно доступный контекст текущего HTTP-запроса.
		/// </summary>
		/// <returns>Текущий контекст запроса <see cref="Microsoft.AspNetCore.Http.HttpContext"/>.</returns>
		public HttpContext HttpContext { get; }


		/// <summary>
		/// Получает фабрику для создания именованных и типизированных HTTP-клиентов.
		/// </summary>
		/// <returns>Фабрика клиентов <see cref="IHttpClientFactory"/>.</returns>
		public IHttpClientFactory HttpClientFactory { get; }


		/// <summary>
		/// Получает службу гибридного кэширования данных <see cref="HybridCache"/>.
		/// </summary>
		/// <returns>Служба кэширования <see cref="HybridCache"/>.</returns>
		public HybridCache HybridCache { get; }


		/// <summary>
		/// Получает службу рендеринга Razor-представлений в строки.
		/// </summary>
		/// <returns>Экземпляр сервиса <see cref="IViewRenderService"/>.</returns>
		public IViewRenderService ViewRender { get; }


		/// <summary>
		/// Получает службу отправки электронной почты SMTP.
		/// </summary>
		/// <returns>Экземпляр почтового сервиса <see cref="IMailerService"/>.</returns>
		public IMailerService Mailer { get; }


		/// <summary>
		/// Получает генератор URL-адресов на основе зарегистрированной системы маршрутов.
		/// </summary>
		/// <returns>Экземпляр генератора ссылок <see cref="Microsoft.AspNetCore.Routing.LinkGenerator"/>.</returns>
		public LinkGenerator LinkGenerator { get; }


		/// <summary>
		/// Получает строго типизированные параметры конфигурации веб-библиотеки.
		/// </summary>
		/// <returns>Объект конфигурационных опций <see cref="LibWebOptions"/>.</returns>
		public LibWebOptions Options { get; }


		/// <summary>
		/// Получает информацию о текущей культуре и языковых стандартах потока.
		/// </summary>
		/// <returns>Текущая информация о локали <see cref="CultureInfo"/>.</returns>
		public CultureInfo Culture { get; }


		/// <summary>
		/// Получает экземпляр помощника для работы с форматированием дат и времени.
		/// </summary>
		/// <returns>Экземпляр класса-помощника <see cref="DateTimeHelper"/>.</returns>
		public DateTimeHelper DateTimeHelper { get; }


		/// <summary>
		/// Получает синглтон-строитель карты сайта и актуальные проиндексированные коллекции навигации.
		/// </summary>
		/// <returns>Экземпляр класса <see cref="SitemapBuilder"/>.</returns>
		public SitemapBuilder Sitemap { get; }


		/* data readonly properties */


		/// <summary>
		/// Получает структурированные данные о сетевом адресе хоста и виртуальных путях приложения.
		/// </summary>
		/// <returns>Экземпляр класса данных хоста <see cref="HostData"/>.</returns>
		public HostData Host { get; }


		/// <summary>
		/// Получает структурированные данные о параметрах роутинга текущего входящего HTTP-запроса.
		/// </summary>
		/// <returns>Экземпляр класса данных маршрута <see cref="RequestData"/>.</returns>
		public RequestData Request { get; }


		/// <summary>
		/// Получает управляющий контейнер SEO и Open Graph метатегов текущей страницы.
		/// </summary>
		/// <returns>Экземпляр класса <see cref="MetaData"/>.</returns>
		public MetaData Meta { get; }


		/* services readonly properties */


		/// <summary>
		/// Получает прикладную службу гибридного кэширования текущего контекста.
		/// </summary>
		/// <returns>Экземпляр службы кэширования контекста <see cref="CacheService"/>.</returns>
		public CacheService Cache { get; }


		/// <summary>
		/// Получает службу безопасного чтения и записи файлов Cookie.
		/// </summary>
		/// <returns>Экземпляр файловой службы <see cref="CookiesService"/>.</returns>
		public CookiesService Cookies { get; }


		/// <summary>
		/// Получает службу сетевого анализа подключения и валидации IP-адресов/подсетей.
		/// </summary>
		/// <returns>Экземпляр сетевой службы <see cref="NetworkService"/>.</returns>
		public NetworkService Network { get; }


		/// <summary>
		/// Получает службу для работы со строкой запроса (Query String) и фильтрации URL параметров.
		/// </summary>
		/// <returns>Экземпляр службы параметров строки запроса <see cref="QueryStringService"/>.</returns>
		public QueryStringService QueryString { get; }


		/// <summary>
		/// Получает службу отправки уведомлений и HTML-писем на базе Razor-представлений.
		/// </summary>
		/// <returns>Экземпляр службы отправки уведомлений <see cref="SendService"/>.</returns>
		public SendService Send { get; }


		/// <summary>
		/// Получает службу выполнения запросов к внешним Web API с поддержкой гибридного кэширования.
		/// </summary>
		/// <returns>Экземпляр службы клиентских API <see cref="WebApiService"/>.</returns>
		public WebApiService WebApi { get; }


		/* profiles readonly properties */


		/// <summary>
		/// Профиль текущего сайта.
		/// </summary>
		/// <returns>Связанный профиль сайта <see cref="SiteProfile"/>.</returns>
		public SiteProfile Site { get; }


		/// <summary>
		/// Профиль текущего информационного узла.
		/// </summary>
		/// <returns>Связанный профиль узла <see cref="NodeProfile"/>.</returns>
		public NodeProfile Node { get; }


		/// <summary>
		/// Профиль текущей контентной страницы.
		/// </summary>
		/// <returns>Связанный профиль контентной страницы <see cref="PageProfile"/>.</returns>
		public PageProfile Page { get; }


		/* readonly properties */


		/// <summary>
		/// Получает признак того, что веб-приложение запущено в среде разработки (Development).
		/// </summary>
		/// <value>Значение <see langword="true"/>, если среда выполнения соответствует <see cref="Microsoft.Extensions.Hosting.Environments.Development"/>; иначе — <see langword="false"/>.</value>
		/// <returns><see langword="true"/>, если приложение запущено в режиме разработки; иначе — <see langword="false"/>.</returns>
		public bool IsDevelopment
			=> Env.IsDevelopment();


		/// <summary>
		/// Получает безопасное HTML-представление человекочитаемого заголовка веб-страницы для тега &lt;title&gt; браузера.
		/// </summary>		
		/// <value>Объект <see cref="HtmlString"/>, готовый к выводу в мастер-шаблоне без повторного экранирования.</value>
		/// <returns>Объект <see cref="HtmlString"/>, содержащий иерархический или кастомный заголовок вкладки.</returns>
		public HtmlString BrowserTitleHtml
			=> field ??= new HtmlString(_getBrowserTitle());


		/// <summary>
		/// Возвращает последовательность элементов навигационной цепочки («хлебных крошек»)
		/// от корневого элемента до текущей активной страницы с учетом правил видимости по умолчанию.
		/// </summary>
		/// <returns>Только для чтения список элементов навигации <see cref="IReadOnlyList{NavItem}"/>.</returns>
		public IReadOnlyList<NavItem> Breadcrumbs
			=> GetBreadcrumbs();


		/* properties */


		/// <summary>
		/// Получает или задает имя основного (базового) файла разметки интерфейса сайта по умолчанию.
		/// </summary>
		/// <value>Строка с путем или именем макета Razor (например, <c>"_Layout"</c>), инициализируемая из глобальных настроек библиотеки.</value>
		/// <returns>Имя или путь к базовому файлу разметки.</returns>
		public string DefaultLayout
		{
			get => field ??= Options.DefaultLayout;
			set;
		}


		/// <summary>
		/// Получает или задает имя файла разметки для системных и административных страниц интерфейса.
		/// </summary>
		/// <value>Строка с именем системного макета Razor. Если не задана явно, возвращает значение <see cref="DefaultLayout"/>.</value>
		/// <returns>Имя файла разметки панели управления или административных страниц.</returns>
		public string SystemLayout
		{
			get => field ??= (Options.SystemLayout ?? DefaultLayout);
			set;
		}


		/// <summary>
		/// Получает или задает имя файла разметки для страниц отображения HTTP-ошибок (404, 403, 500 и т.д.).
		/// </summary>
		/// <value>Строка с именем макета Razor для вывода ошибок. Если не задана явно, возвращает значение <see cref="DefaultLayout"/>.</value>
		/// <returns>Имя файла макета Razor для страниц ошибок.</returns>
		public string ErrorsLayout
		{
			get => field ??= (Options.Errors.Layout ?? DefaultLayout);
			set;
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


		/* functions */


		/// <summary>
		/// Формирует и кэширует в рамках текущего запроса цепочку «хлебных крошек»
		/// для заданного набора активных тегов навигатора.
		/// </summary>
		/// <param name="tags">Опциональный набор тегов контекста (например, <c>"science"</c>, <c>"breadcrumbs"</c>).</param>
		/// <returns>Упорядоченный сверху вниз список элементов <see cref="IReadOnlyList{NavItem}"/>.</returns>
		public IReadOnlyList<NavItem> GetBreadcrumbs(
			params string[]? tags)
		{
			string key1 = _getTagsCacheKey(tags);
			_breadcrumbsCache ??= new Dictionary<string, IReadOnlyList<NavItem>>(StringComparer.Ordinal);
			if (_breadcrumbsCache.TryGetValue(key1, out var cached1))
				return cached1;

			var result1 = _buildBreadcrumbs(tags);
			_breadcrumbsCache[key1] = result1;
			return result1;
		}


		/// <summary>
		/// Формирует и кэширует в рамках текущего запроса иерархическое дерево узлов сайта
		/// с фильтрацией по тегам и автоматическим схлопыванием сквозных элементов и пустых групп.
		/// </summary>
		/// <param name="maxDepth">Максимальная глубина вложенности (1 — только верхний уровень, <see langword="null"/> — без ограничений).</param>
		/// <param name="tags">Опциональный набор активных тегов навигатора.</param>
		/// <returns>Список корневых элементов дерева узлов <see cref="IReadOnlyList{NavItem}"/>.</returns>
		public IReadOnlyList<NavItem> GetNodesNav(
			int? maxDepth = null,
			params string[]? tags)
		{
			string key1 = $"nodes|{maxDepth}|{_getTagsCacheKey(tags)}";
			_navCache ??= new Dictionary<string, IReadOnlyList<NavItem>>(StringComparer.Ordinal);
			if (_navCache.TryGetValue(key1, out var cached1))
				return cached1;

			var activeNodes1 = _getActiveNodeAncestors();
			var result1 = _buildNodesTree(Sitemap.TopNodes, 1, maxDepth, tags, activeNodes1);
			_navCache[key1] = result1;
			return result1;
		}


        /// <summary>
        /// Формирует и кэширует в рамках текущего запроса дерево дочерних узлов текущего информационного узла
        /// с фильтрацией по тегам и автоматическим схлопыванием сквозных элементов и пустых групп.
        /// </summary>
        /// <param name="maxDepth">Максимальная глубина вложенности (1 — только прямые потомки, <see langword="null"/> — без ограничений).</param>
        /// <param name="tags">Опциональный набор активных тегов навигатора.</param>
        /// <returns>Список дочерних узлов <see cref="IReadOnlyList{NavItem}"/>.</returns>
        public IReadOnlyList<NavItem> GetSubnodesNav(
            int? maxDepth = null,
            params string[]? tags)
        {
            if (Node.MapItem is null || !Node.MapItem.HasChildNodes)
                return [];

            string key1 = $"subnodes|{Node.MapItem.Name}|{maxDepth}|{_getTagsCacheKey(tags)}";
            _navCache ??= new Dictionary<string, IReadOnlyList<NavItem>>(StringComparer.Ordinal);
            if (_navCache.TryGetValue(key1, out var cached1))
                return cached1;

            var activeNodes1 = _getActiveNodeAncestors();
            var result1 = _buildNodesTree(Node.MapItem.ChildNodes, 1, maxDepth, tags, activeNodes1);
            _navCache[key1] = result1;
            return result1;
        }


        /// <summary>
        /// Формирует и кэширует в рамках текущего запроса дерево страниц текущего информационного узла
        /// с фильтрацией по тегам и автоматическим схлопыванием сквозных элементов.
        /// </summary>
        /// <param name="maxDepth">Максимальная глубина вложенности (<see langword="null"/> — без ограничений).</param>
        /// <param name="tags">Опциональный набор активных тегов навигатора.</param>
        /// <returns>Список корневых страниц узла <see cref="IReadOnlyList{NavItem}"/>.</returns>
        public IReadOnlyList<NavItem> GetPagesNav(
			int? maxDepth = null,
			params string[]? tags)
		{
			if (Node.MapItem is null)
				return [];

			string key1 = $"pages|{Node.MapItem.Name}|{maxDepth}|{_getTagsCacheKey(tags)}";
			_navCache ??= new Dictionary<string, IReadOnlyList<NavItem>>(StringComparer.Ordinal);
			if (_navCache.TryGetValue(key1, out var cached1))
				return cached1;

			var activePages1 = _getActivePageAncestors();
			var result1 = _buildPagesTree(Node.MapItem.TopPages, 1, maxDepth, tags, activePages1);
			_navCache[key1] = result1;
			return result1;
		}


		/// <summary>
		/// Формирует и кэширует в рамках текущего запроса дерево дочерних подстраниц текущей активной страницы.
		/// </summary>
		/// <param name="maxDepth">Максимальная глубина вложенности (<see langword="null"/> — без ограничений).</param>
		/// <param name="tags">Опциональный набор активных тегов навигатора.</param>
		/// <returns>Список дочерних подстраниц <see cref="IReadOnlyList{NavItem}"/>.</returns>
		public IReadOnlyList<NavItem> GetSubpagesNav(
			int? maxDepth = null,
			params string[]? tags)
		{
			if (Page.MapItem is null || !Page.MapItem.HasChildPages)
				return [];

			string key1 = $"subpages|{Page.MapItem.Path}|{maxDepth}|{_getTagsCacheKey(tags)}";
			_navCache ??= new Dictionary<string, IReadOnlyList<NavItem>>(StringComparer.Ordinal);
			if (_navCache.TryGetValue(key1, out var cached1))
				return cached1;

			var activePages1 = _getActivePageAncestors();
			var result1 = _buildPagesTree(Page.MapItem.ChildPages, 1, maxDepth, tags, activePages1);
			_navCache[key1] = result1;
			return result1;
		}


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
		/// Формирует полный URL-адрес статического ресурса на основе переданной семантической строки назначения.
		/// </summary>
		/// <param name="target">Строка назначения ресурса или прямой URL.</param>
		/// <returns>Полный URL-адрес для вывода в тегах разметки (например, изображений, скриптов, стилей).</returns>
		public string GetResUrl(
			string? target)
		{
			if (string.IsNullOrEmpty(target))
				return string.Empty;
			if (target[0] == '/')
				return $"{Site.ResUrl}{target}";
			if (target.StartsWith("site:"))
				return $"{Site.ResUrl}{target[5..].Make("/{0}")}";
			if (target.StartsWith("node:"))
				return $"{Node.ResUrl}{target[5..].Make("/{0}")}";
			if (target.StartsWith("page:"))
				return $"{Page.ResUrl}{target[5..].Make("/{0}")}";
			return target;
		}


		/// <summary>
		/// Формирует интеллектуальный URL-адрес перехода на основе префиксов доменной области (site:, node:, page:).
		/// </summary>
		/// <param name="target">Строка целевого назначения ссылки или относительный ЧПУ-маршрут.</param>
		/// <returns>Канонический виртуальный путь к контентному элементу веб-приложения.</returns>
		public string GetSmartUrl(
			string? target)
		{
			if (string.IsNullOrEmpty(target))
				return string.Empty;
			if (target[0] == '/')
				return $"{Host.VirtualPath}{target}";
			if (target.StartsWith("site:"))
				return $"{Site.Url}/{target[5..]}";
			if (target.StartsWith("node:"))
				return $"{Node.Url}/{target[5..]}";
			if (target.StartsWith("page:"))
				return $"{Page.Url}/{target[5..]}";
			return target;
		}


		/* privates */


		internal void _ResetBreadcrumbsCache()
		{
			_breadcrumbsCache?.Clear();
		}


		private string _getBrowserTitle()
		{
			if (!string.IsNullOrWhiteSpace(Page.CustomBrowserTitle))
				return Page.CustomBrowserTitle;
			return $"{Page.GetParentsTitles()}{Page.ShortTitle.Make("{0} – ")}{Node.ShortTitle.Make("{0} – ")}{Site.ShortTitle}";
		}


		private static string _getTagsCacheKey(
			string[]? tags)
		{
			if (tags is not { Length: > 0 })
				return string.Empty;
			return string.Join(',', tags.Select(x => x.ToLowerInvariant()));
		}


		private ReadOnlyCollection<NavItem> _buildBreadcrumbs(
			string[]? tags)
		{
			var rawChain1 = new List<NavItem>();
			rawChain1.AddRange(Site.GetBreadcrumbsPart());
			rawChain1.AddRange(Node.GetBreadcrumbsPart());
			rawChain1.AddRange(Page.GetBreadcrumbsPart());

			if (rawChain1 is [])
				return [];

			var result1 = new List<NavItem>(rawChain1.Count);
			int lastIndex1 = rawChain1.Count - 1;

			for (int i = lastIndex1; i >= 0; i--)
			{
				var item1 = rawChain1[i];
				bool isCurrent1 = i == lastIndex1;

				if (!item1.IsDisplayed(tags))
				{
					if (isCurrent1 && !item1.IsGroup && !item1.IsPassthrough(tags))
						result1.Add(item1);
					break;
				}

				if (item1.IsGroup || item1.IsPassthrough(tags))
					continue;

				result1.Add(item1);
			}

			result1.Reverse();
			return result1.AsReadOnly();
		}


		private HashSet<NodemapItem> _getActiveNodeAncestors()
		{
			var set1 = new HashSet<NodemapItem>();
			var current1 = Node.MapItem;
			while (current1 is not null)
			{
				set1.Add(current1);
				current1 = current1.ParentNode;
			}
			return set1;
		}


		private HashSet<PagemapItem> _getActivePageAncestors()
		{
			var set1 = new HashSet<PagemapItem>();
			var current1 = Page.MapItem;
			while (current1 is not null)
			{
				set1.Add(current1);
				current1 = current1.ParentPage;
			}
			return set1;
		}


		private ReadOnlyCollection<NavItem> _buildNodesTree(
			IEnumerable<NodemapItem> nodes,
			int depth,
			int? maxDepth,
			string[]? tags,
			HashSet<NodemapItem> activeNodes)
		{
			if (maxDepth is not null && depth > maxDepth.Value)
				return [];

			var result1 = new List<NavItem>();
			foreach (var node1 in nodes)
			{
				if (!node1.IsDisplayed(tags))
					continue;

				if (node1.IsPassthrough(tags))
				{
					if (node1.HasChildNodes)
						result1.AddRange(_buildNodesTree(
							node1.ChildNodes, depth, maxDepth, tags, activeNodes));
					continue;
				}

				IReadOnlyList<NavItem> children1 = node1.HasChildNodes
					? _buildNodesTree(node1.ChildNodes, depth + 1, maxDepth, tags, activeNodes)
					: [];

				if (node1.Type is NodemapTypesEnum.Group && children1 is [])
					continue;

				bool isActive1 = ReferenceEquals(node1, Node.MapItem);
				bool isActiveBranch1 = !isActive1 && activeNodes.Contains(node1);

				result1.Add(new NavItem(
					node1,
					HttpContext,
					isActive: isActive1,
					isActiveBranch: isActiveBranch1,
					children: children1));
			}

			return result1.AsReadOnly();
		}


		private ReadOnlyCollection<NavItem> _buildPagesTree(
			IEnumerable<PagemapItem> pages,
			int depth,
			int? maxDepth,
			string[]? tags,
			HashSet<PagemapItem> activePages)
		{
			if (maxDepth is not null && depth > maxDepth.Value)
				return [];

			var result1 = new List<NavItem>();
			foreach (var page1 in pages)
			{
				if (!page1.IsDisplayed(tags))
					continue;

				if (page1.IsPassthrough(tags))
				{
					if (page1.HasChildPages)
						result1.AddRange(_buildPagesTree(
							page1.ChildPages, depth, maxDepth, tags, activePages));
					continue;
				}

				IReadOnlyList<NavItem> children1 = page1.HasChildPages
					? _buildPagesTree(page1.ChildPages, depth + 1, maxDepth, tags, activePages)
					: [];

				bool isActive1 = ReferenceEquals(page1, Page.MapItem);
				bool isActiveBranch1 = !isActive1 && activePages.Contains(page1);

				result1.Add(new NavItem(
					page1,
					HttpContext,
					isActive: isActive1,
					isActiveBranch: isActiveBranch1,
					children: children1));
			}

			return result1.AsReadOnly();
		}

	}

}
