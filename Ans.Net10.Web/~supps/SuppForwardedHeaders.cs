// rev 2026-09-30

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

namespace Ans.Net10.Web
{

	/*
	
		При регистрации сервисов (DI):
		builder.Services.configure<ForwardedHeadersOptions>(options =>
		{
			SuppForwardedHeaders.Configure(options, webOptions);
		});

		В самом начале конвейера middleware (до вызова UseRouting и UseAuthorization):
		// Это middleware должно стоять ПЕРВЫМ в приложении!
		app.UseForwardedHeaders();

	 */


	/// <summary>
	/// Вспомогательный класс для настройки интеграции веб-приложения с обратными прокси-серверами (Nginx, Apache, IIS).
	/// </summary>
	public static class SuppForwardedHeaders
	{

		/// <summary>
		/// Настраивает параметры обработки прокси-заголовков на основе конфигурации библиотеки, 
		/// используя современные типы данных платформы .NET 10.
		/// </summary>
		/// <param name="options">Системные опции ForwardedHeaders платформы.</param>
		/// <param name="webOptions">Загруженные параметры конфигурации библиотеки.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметры <paramref name="options"/> или <paramref name="webOptions"/> равны <see langword="null"/>.
		/// </exception>
		public static void Configure(
			ForwardedHeadersOptions options,
			LibWebOptions webOptions)
		{
			ArgumentNullException.ThrowIfNull(options);
			ArgumentNullException.ThrowIfNull(webOptions);
			if (webOptions.Proxy?.UseForwardedHeaders != true)
				return;
			options.ForwardedHeaders =
				ForwardedHeaders.XForwardedFor
				| ForwardedHeaders.XForwardedProto
				| ForwardedHeaders.XForwardedHost;
			options.KnownIPNetworks.Clear();
			options.KnownProxies.Clear();
			var knownProxiesStr1 = webOptions.Proxy.KnownProxies;

			if (!string.IsNullOrEmpty(knownProxiesStr1))
			{
				var ips1 = knownProxiesStr1.Split(
					';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				foreach (var s1 in ips1)
					if (IPAddress.TryParse(s1, out var ip1))
						options.KnownProxies.Add(ip1);
					else if (System.Net.IPNetwork.TryParse(s1, out var network1))
						options.KnownIPNetworks.Add(network1);
			}
			else
			{
				options.KnownProxies.Add(IPAddress.Loopback);
				options.KnownProxies.Add(IPAddress.IPv6Loopback);
			}
		}

	}

}
