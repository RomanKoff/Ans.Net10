// rev 2026-11-15

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
		/// <param name="forwardedHeadersOptions">Системные опции ForwardedHeaders платформы.</param>
		/// <param name="libWebAppOptions">Загруженные параметры конфигурации библиотеки.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметры <paramref name="forwardedHeadersOptions"/> или <paramref name="libWebAppOptions"/> равны <see langword="null"/>.
		/// </exception>
		public static void Configure(
			ForwardedHeadersOptions forwardedHeadersOptions,
			LibWebOptions libWebAppOptions)
		{
			ArgumentNullException.ThrowIfNull(forwardedHeadersOptions);
			ArgumentNullException.ThrowIfNull(libWebAppOptions);
			if (libWebAppOptions.Proxy?.UseForwardedHeaders != true)
				return;
			forwardedHeadersOptions.ForwardedHeaders =
				ForwardedHeaders.XForwardedFor
				| ForwardedHeaders.XForwardedProto
				| ForwardedHeaders.XForwardedHost;
			forwardedHeadersOptions.KnownIPNetworks.Clear();
			forwardedHeadersOptions.KnownProxies.Clear();
			var knownProxiesStr1 = libWebAppOptions.Proxy.KnownProxies;

			if (!string.IsNullOrEmpty(knownProxiesStr1))
			{
				var ips1 = knownProxiesStr1.Split(
					';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				foreach (var s1 in ips1)
					if (IPAddress.TryParse(s1, out var ip1))
						forwardedHeadersOptions.KnownProxies.Add(ip1);
					else if (System.Net.IPNetwork.TryParse(s1, out var network1))
						forwardedHeadersOptions.KnownIPNetworks.Add(network1);
			}
			else
			{
				forwardedHeadersOptions.KnownProxies.Add(IPAddress.Loopback);
				forwardedHeadersOptions.KnownProxies.Add(IPAddress.IPv6Loopback);
			}
		}

	}

}
