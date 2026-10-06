// rev 2026-10-04

using Ans.Net10.Common;
using Ans.Net10.Common.Services;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Параметры конфигурации для библиотеки <c>Ans.Net10.Web</c>.
	/// </summary>
	public class LibWebOptions
		: _AppSettings_Base
	{

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="LibWebOptions"/> 
		/// с системным именем секции по умолчанию.
		/// </summary>
		public LibWebOptions()
			: base("Ans.Net10.Web")
		{
		}


		/// <inheritdoc />
		/// <remarks>
		/// Выполняет каскадную валидацию всех дочерних секций параметров, если они были инициализированы.
		/// </remarks>
		public override void Test()
		{
			if (Region == null)
				throw GetExceptionParamRequired("Region");
			if (string.IsNullOrEmpty(Region.Culture))
				throw GetExceptionParamRequired("Region.Culture");
			if (string.IsNullOrEmpty(Region.RegionPhoneCode))
				throw GetExceptionParamRequired("Region.TelCode");

			if (UI == null)
				throw GetExceptionParamRequired(nameof(UI));
			if (string.IsNullOrEmpty(UI.DefaultLayout))
				throw GetExceptionParamRequired("UI.DefaultLayout");
		}


		/// <summary>
		/// Получает или задает флаг работы приложения в режиме Content-driven.
		/// </summary>
		public bool UseContentDrivenMode { get; set; } = false;


		/// <summary>
		/// Получает или задает флаг включения Runtime-компиляции для представлений Razor.
		/// </summary>
		public bool UseRuntimeCompilation { get; set; } = false;


		/// <summary>
		/// Получает или задает флаг режима разработки.
		/// </summary>
		public bool UseDeveloperMode { get; set; } = false;


		/// <summary>
		/// Получает или задает системный токен безопасности приложения.
		/// </summary>
		public string? SystemToken { get; set; }


		/// <summary>
		/// Получает или задает список переопределений MIME-типов.
		/// </summary>
		public string[]? Mimetypes { get; set; }


		/// <summary>
		/// Получает или задает список кастомных маршрутов роутинга.
		/// </summary>
		public string[]? Routes { get; set; }


		/// <summary>
		/// Получает или задает региональные настройки.
		/// </summary>
		public RegionOptions? Region { get; set; }


		/// <summary>
		/// Получает или задает параметры конфигурации графического пользовательского интерфейса (UI).
		/// </summary>
		/// <remarks>
		/// Секция инкапсулирует глобальные настройки визуального отображения веб-приложения, 
		/// включая базовые CSS-классы контейнеров и пути к мастер-шаблонам разметки (Layouts).
		/// </remarks>
		public UIOptions? UI { get; set; }


		/// <summary>
		/// Получает или задает настройки отображения и обработки ошибок.
		/// </summary>
		public ErrorsOptions? Errors { get; set; }


		/// <summary>
		/// Получает или задает параметры обработки заголовков обратного прокси-сервера (Nginx/IIS).
		/// </summary>
		public ProxyOptions? Proxy { get; set; }


		/// <summary>
		/// Получает или задает параметры конфигурации политик CORS (Cross-Origin Resource Sharing).
		/// </summary>
		/// <remarks>
		/// Настройки используются сервисом <see cref="SuppCors"/> для регистрации и последующей 
		/// активации именованных профилей междоменной безопасности в конвейере обработки запросов.
		/// </remarks>
		public CorsOptions? Cors { get; set; }


		/// <summary>
		/// Получает или задает параметры службы отправки писем.
		/// </summary>
		public MailServiceOptions? MailService { get; set; }


		/// <summary>
		/// Получает или задает списки подсетей для ограничения доступа.
		/// </summary>
		public SubnetsOptions? Subnets { get; set; }

	}



	/// <summary>
	/// Региональные настройки.
	/// </summary>
	public class RegionOptions
	{

		/// <summary>
		/// Получает или задает культуру по умолчанию (например, "ru", "en").
		/// </summary>
		public string Culture { get; set; } = "ru";


		/// <summary>
		/// Получает или задает телефонный код региона по умолчанию.
		/// </summary>
		public string RegionPhoneCode { get; set; } = "7812";

	}



	/// <summary>
	/// Описывает параметры конфигурации графического пользовательского интерфейса 
	/// и глобальных шаблонов разметки веб-приложения.
	/// </summary>
	public class UIOptions
	{
		/// <summary>
		/// Получает или задает имя основного файла мастер-шаблона разметки (Layout) по умолчанию.
		/// </summary>
		/// <value>
		/// Строковое имя или относительный путь к файлу представления (например, <c>"_Layout"</c>).
		/// Значение по умолчанию: <c>"_Layout"</c>.
		/// </value>
		public string DefaultLayout { get; set; } = "_Layout";

		/// <summary>
		/// Получает или задает имя глобального альтернативного или системного шаблона разметки.
		/// </summary>
		/// <value>
		/// Строковое имя файла шаблона или <see langword="null"/>, если системный шаблон разметки не используется.
		/// </value>
		public string? SystemLayout { get; set; }

		/// <summary>
		/// Получает или задает базовый набор CSS-классов для главного оборачивающего HTML-контейнера страниц.
		/// </summary>
		/// <value>
		/// Строка с перечислением CSS-классов (например, в стиле фреймворка Bootstrap 5: <c>"container"</c> или <c>"container-fluid"</c>).
		/// Значение по умолчанию: <c>"container"</c>.
		/// </value>
		public string DefaultContainerClasses { get; set; } = "container";
	}



	/// <summary>
	/// Параметры конфигурации страниц отображения ошибок и статусных кодов HTTP.
	/// </summary>
	public class ErrorsOptions
	{
		/// <summary>
		/// Получает или задает кастомный Layout для страниц ошибок.
		/// </summary>
		public string? Layout { get; set; }

		/// <summary>
		/// Получает или задает относительный путь к единой универсальной Razor-странице фреймворка 
		/// для отображения как фатальных серверных исключений (500), так и HTTP статус-кодов (404, 403, 400).
		/// </summary>
		/// <value>
		/// Строковый виртуальный путь к эндпоинту (например, <c>"/Ans/Errors"</c>).
		/// </value>
		public string? RazorPageErrorsPath { get; set; }

		/// <summary>
		/// Получает или задает флаг вывода детальной технической информации об ошибке.
		/// </summary>
		public bool ShowInfo { get; set; }

		/// <summary>
		/// Получает или задает URL/путь к изображению для ошибки 400 Bad Request.
		/// </summary>
		public string? Picture400 { get; set; }

		/// <summary>
		/// Получает или задает URL/путь к изображению для ошибки 403 Forbidden.
		/// </summary>
		public string? Picture403 { get; set; }

		/// <summary>
		/// Получает или задает URL/путь к изображению для ошибки 404 Not Found.
		/// </summary>
		public string? Picture404 { get; set; }

		/// <summary>
		/// Получает или задает URL/путь к изображению для ошибки 500 Internal Server Error.
		/// </summary>
		public string? Picture500 { get; set; }
	}



	/// <summary>
	/// Параметры конфигурации обработки прокси-заголовков.
	/// </summary>
	public class ProxyOptions
	{
		/// <summary>
		/// Флаг принудительного включения поддержки прокси-заголовков Forwarded Headers.
		/// </summary>
		public bool UseForwardedHeaders { get; set; } = false;

		/// <summary>
		/// Список доверенных IP-адресов прокси-серверов (например, "127.0.0.1;192.168.1.10").
		/// Если пустой — по умолчанию доверяется локальной петле (Loopback).
		/// </summary>
		public string KnownProxies { get; set; } = string.Empty;
	}



	/// <summary>
	/// Описывает параметры конфигурации совместного использования ресурсов между разными источниками (CORS) 
	/// для разграничения прав доступа к веб-приложению со стороны браузеров.
	/// </summary>
	public class CorsOptions
	{

		/// <summary>
		/// Получает или задает имя активного профиля CORS, применяемого ко всему приложению по умолчанию.
		/// </summary>
		/// <value>
		/// Строковое имя профиля (например, <c>"ALLOW ALL"</c>, <c>"ALLOW LOCAL"</c> или <c>"ALLOW CREDENTIALS"</c>).
		/// </value>
		/// <remarks>
		/// Допустимые стандартные константы профилей определены в классе <see cref="SuppCors"/>. 
		/// Если профиль не указан или равен <see langword="null"/>, фреймворк автоматически активирует профиль защиты по умолчанию.
		/// </remarks>
		public string? Profile { get; set; }


		/// <summary>
		/// Получает или задает коллекцию локальных адресов источников (Origins) разработчиков, 
		/// используемых для междоменной отладки клиентских интерфейсов.
		/// </summary>
		/// <value>
		/// Массив строковых URL-адресов локальных хостов (например, <c>["http://localhost:3000", "http://localhost:5173"]</c>).
		/// </value>
		public string[]? LocalOrigins { get; set; }


		/// <summary>
		/// Получает или задает коллекцию доверенных корпоративных или внешних адресов источников (Origins), 
		/// которым разрешено выполнять междоменные запросы к ресурсам приложения в режиме Production.
		/// </summary>
		/// <value>
		/// Массив официальных строковых URL-адресов доверенных доменов (например, <c>["https://my-company.ru"]</c>).
		/// </value>
		/// <remarks>
		/// Данный список является обязательным для корректной работы политик безопасности 
		/// <see cref="SuppCors.CORS_ALLOW_TRUSTED"/> и <see cref="SuppCors.CORS_ALLOW_CREDENTIALS"/>.
		/// </remarks>
		public string[]? TrustedOrigins { get; set; }

	}



	/// <summary>
	/// Параметры конфигурации почтовой службы, реализующие интерфейс <see cref="IMailerServiceOptions"/> 
	/// из инфраструктуры библиотеки <c>Ans.Net10.Common</c>.
	/// </summary>
	public class MailServiceOptions
		: IMailerServiceOptions
	{
		/// <inheritdoc />
		public string SmtpServer { get; set; } = string.Empty;

		/// <inheritdoc />
		public int SmtpPort { get; set; }

		/// <inheritdoc />
		public bool SmtpUseSsl { get; set; }

		/// <inheritdoc />
		public string SmtpUsername { get; set; } = string.Empty;

		/// <inheritdoc />
		public string SmtpPassword { get; set; } = string.Empty;

		/// <inheritdoc />
		public string DefaultFromAddress { get; set; } = string.Empty;

		/// <inheritdoc />
		public string DefaultFromTitle { get; set; } = string.Empty;

		/// <inheritdoc />
		public string DebugCc { get; set; } = string.Empty;
	}



	/// <summary>
	/// Конфигурация списков IP-подсетей с ленивой инициализацией типизированных CIDR-структур.
	/// </summary>
	public class SubnetsOptions
	{

		/* options */


		/// <summary>
		/// Получает или задает сырую CIDR-строку подсетей администраторов.
		/// </summary>
		public string Admin { get; set; } = string.Empty;

		/// <summary>
		/// Получает или задает сырую CIDR-строку безопасных подсетей.
		/// </summary>
		public string Safe { get; set; } = string.Empty;

		/// <summary>
		/// Получает или задает сырую CIDR-строку небезопасных подсетей.
		/// </summary>
		public string Unsafe { get; set; } = string.Empty;

		/// <summary>
		/// Получает или задает сырую CIDR-строку разрешенных подсетей.
		/// </summary>
		public string Allow { get; set; } = string.Empty;

		/// <summary>
		/// Получает или задает сырую CIDR-строку запрещенных подсетей.
		/// </summary>
		public string Deny { get; set; } = string.Empty;


		/* readonly properties */


		/// <summary>
		/// Возвращает типизированный список подсетей администраторов с ленивой инициализацией.
		/// </summary>
		public IPSubnetsList? AdminSubnets
			=> _getSubnets(Admin, ref field);

		/// <summary>
		/// Возвращает типизированный список безопасных подсетей с ленивой инициализацией.
		/// </summary>
		public IPSubnetsList? SafeSubnets
			=> _getSubnets(Safe, ref field);

		/// <summary>
		/// Возвращает типизированный список небезопасных подсетей с ленивой инициализацией.
		/// </summary>
		public IPSubnetsList? UnsafeSubnets
			=> _getSubnets(Unsafe, ref field);

		/// <summary>
		/// Возвращает типизированный список разрешенных подсетей с ленивой инициализацией.
		/// </summary>
		public IPSubnetsList? AllowSubnets
			=> _getSubnets(Allow, ref field);

		/// <summary>
		/// Возвращает типизированный список запрещенных подсетей с ленивой инициализацией.
		/// </summary>
		public IPSubnetsList? DenySubnets
			=> _getSubnets(Deny, ref field);


		/* privates */


		private static IPSubnetsList? _getSubnets(
			string value,
			ref IPSubnetsList? cache)
		{
			return string.IsNullOrEmpty(value)
				? null
				: cache ??= new IPSubnetsList(value);
		}

	}

}
