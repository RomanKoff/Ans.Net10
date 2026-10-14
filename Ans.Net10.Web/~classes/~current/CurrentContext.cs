// rev 2026-10-14

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
		/// <param name="loggerFactory">Фабрика для создания системных регистраторов логов.</param>
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
		/// <exception cref="Exception">
		/// Вызывается при критическом сбое системы, если текущий контекст <see cref="HttpContext"/> недоступен через аксессор.
		/// </exception>
		// <param name="sitemap">Поставщик данных и управления жизненным циклом карты сайта.</param>
		public CurrentContext(
			ILoggerFactory loggerFactory,
			IWebHostEnvironment env,
			IConfiguration configuration,
			IHttpContextAccessor httpContextAccessor,
			HybridCache hybridCache,
			IViewRenderService viewRender,
			IHttpClientFactory httpClientFactory,
			//ISitemapProvider sitemap,
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

			Logger = loggerFactory.CreateLogger("Ans.Net10.Web");
			Env = env;
			Configuration = configuration;
			HttpContext = httpContextAccessor.HttpContext
				?? throw new Exception("HttpContext is busy!");
			HybridCache = hybridCache;
			ViewRender = viewRender;
			HttpClientFactory = httpClientFactory;
			//Sitemap = sitemap;
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
		//public ISitemapProvider Sitemap { get; }


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
			=> field ??= Site is null
				? null : _getBreadcrumbsLazy();


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
			return $"{Host.VirtualPath}/{target.TrimStart('/')}";
		}


		/// <summary>
		/// Формирует собранный человекочитаемый заголовок веб-страницы для тега &lt;title&gt; браузера.
		/// Объединяет иерархию сокращенных названий страницы, узла и сайта.
		/// </summary>
		/// <value>Строка готового заголовка окна браузера.</value>
		public string GetBrowserTitle()
		{
			if (Page is not null && !string.IsNullOrWhiteSpace(Page.CustomBrowserTitle))
				return Page.CustomBrowserTitle;
			string pageTitle1 = Page?.ShortTitle ?? string.Empty;
			string nodeTitle1 = Node?.ShortTitle ?? string.Empty;
			string siteTitle1 = Site?.ShortTitle ?? string.Empty;
			if (!string.IsNullOrEmpty(pageTitle1) && !string.IsNullOrEmpty(nodeTitle1))
				return $"{pageTitle1} – {nodeTitle1} – {siteTitle1}";
			if (!string.IsNullOrEmpty(nodeTitle1))
				return $"{nodeTitle1} – {siteTitle1}";
			return siteTitle1;
		}


		/* todo */


		/// <summary>
		/// Возвращает URL-адрес изолированного ресурса на основе переданного идентификатора.
		/// </summary>
		/// <param name="target">Идентификатор или путь ресурса.</param>
		/// <returns>Строка сформированного URL-адреса ресурса.</returns>
		/// <exception cref="NotImplementedException">Метод будет реализован в следующих ревизиях.</exception>
		internal string GetResUrl(
			string target)
		{
			throw new NotImplementedException();
		}


		/// <summary>
		/// Возвращает внутренний маршрут на основе переданного строкового кода.
		/// </summary>
		/// <param name="target">Строковое определение целевого пути.</param>
		/// <returns>Строка сформированного внутреннего URL.</returns>
		/// <exception cref="NotImplementedException">Метод будет реализован в следующих ревизиях.</exception>
		internal string GetUrl(
			string target)
		{
			throw new NotImplementedException();
		}


		/* privates */



		private IEnumerable<LinkBuilder> _getBreadcrumbsLazy()
		{
			var rootLink1 = new LinkBuilder(Site.HomeUrl, Site.ShortTitle);
			if (Node is null && Page is null)
			{
				rootLink1.IsDisabled = true;
				yield return rootLink1;
				yield break;
			}
			yield return rootLink1;
			if (Node is not null)
			{
				var nodesChain1 = new List<NodeProfile>();
				var currentNode1 = Node;
				while (currentNode1 is not null)
				{
					nodesChain1.Add(currentNode1);
					currentNode1 = currentNode1.ParentNode;
				}
				for (int i1 = nodesChain1.Count - 1; i1 >= 0; i1--)
				{
					var node1 = nodesChain1[i1];
					string? nodeHref1 = node1.Type == NodeProfileType.Group
						? null : node1.Name;
					var nodeLink1 = new LinkBuilder(nodeHref1, node1.ShortTitle);
					if (Page is null && i1 == 0)
					{
						nodeLink1.IsDisabled = true;
						yield return nodeLink1;
						yield break;
					}
					yield return nodeLink1;
				}
			}
			if (Page is not null)
			{
				var pagesChain1 = new List<PageProfile>();
				var currentPage1 = Page;
				while (currentPage1 is not null)
				{
					pagesChain1.Add(currentPage1);
					currentPage1 = currentPage1.ParentPage;
				}
				for (int i1 = pagesChain1.Count - 1; i1 >= 0; i1--)
				{
					var page1 = pagesChain1[i1];
					var pageLink1 = new LinkBuilder(page1.Path, page1.ShortTitle);
					if (i1 == 0)
						pageLink1.IsDisabled = true;
					yield return pageLink1;
				}
			}
		}

	}

}
