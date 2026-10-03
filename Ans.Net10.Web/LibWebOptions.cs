// rev 2026-09-30

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
		/// Получает или задает имя глобального шаблона разметки (Layout) по умолчанию.
		/// </summary>
		public string? SystemLayout { get; set; }


		/// <summary>
		/// Получает или задает имя профиля CORS.
		/// </summary>
		public string? CorsProfile { get; set; }


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
		/// Получает или задает параметры службы отправки писем.
		/// </summary>
		public MailServiceOptions? MailService { get; set; }


		/// <summary>
		/// Получает или задает настройки отображения и обработки ошибок.
		/// </summary>
		public ErrorsOptions? Errors { get; set; }


		/// <summary>
		/// Получает или задает списки подсетей для ограничения доступа.
		/// </summary>
		public SubnetsOptions? Subnets { get; set; }


		/// <summary>
		/// Получает или задает параметры обработки заголовков обратного прокси-сервера (Nginx/IIS).
		/// </summary>
		public ProxyOptions? Proxy { get; set; }

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
	/// Параметры конфигурации страниц отображения ошибок и статусных кодов HTTP.
	/// </summary>
	public class ErrorsOptions
	{
		/// <summary>
		/// Получает или задает кастомный Layout для страниц ошибок.
		/// </summary>
		public string Layout { get; set; } = string.Empty;

		/// <summary>
		/// Получает или задает путь к странице фатальной ошибки сервера (500).
		/// </summary>
		public string ServerErrorPath { get; set; } = string.Empty;

		/// <summary>
		/// Получает или задает путь к странице ошибок обработки HTTP-статусов.
		/// </summary>
		public string HttpErrorPath { get; set; } = string.Empty;

		/// <summary>
		/// Получает или задает флаг вывода детальной технической информации об ошибке.
		/// </summary>
		public bool ShowInfo { get; set; }

		/// <summary>
		/// Получает или задает URL/путь к изображению для ошибки 400 Bad Request.
		/// </summary>
		public string Picture400 { get; set; } = string.Empty;

		/// <summary>
		/// Получает или задает URL/путь к изображению для ошибки 403 Forbidden.
		/// </summary>
		public string Picture403 { get; set; } = string.Empty;

		/// <summary>
		/// Получает или задает URL/путь к изображению для ошибки 404 Not Found.
		/// </summary>
		public string Picture404 { get; set; } = string.Empty;

		/// <summary>
		/// Получает или задает URL/путь к изображению для ошибки 500 Internal Server Error.
		/// </summary>
		public string Picture500 { get; set; } = string.Empty;
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

}
