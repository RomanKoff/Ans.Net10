// rev 2026-09-30

namespace Ans.Net10.Web
{

	/// <summary>
	/// Вспомогательный класс для централизованного конфигурирования, каталогизации 
	/// и регистрации профилей безопасности CORS (Cross-Origin Resource Sharing).
	/// </summary>
	public static class SuppCors
	{

		/* consts */


		/// <summary>
		/// Системное имя CORS-профиля, разрешающего любые входящие междоменные запросы.
		/// </summary>
		public const string CORS_ALLOW_ALL = "ALLOW ALL";


		/// <summary>
		/// Системное имя CORS-профиля, разрешающего доступ исключительно списку доверенных внутренних 
		/// или корпоративных доменов (Origins), указанных в конфигурации приложения.
		/// </summary>
		public const string CORS_ALLOW_TRUSTED = "ALLOW TRUSTED";


		/// <summary>
		/// Системное имя CORS-профиля, разрешающего запросы с локальных адресов разработчиков 
		/// (например, localhost или 127.0.0.1) для обеспечения отладки фронтенд-приложений.
		/// </summary>
		public const string CORS_ALLOW_LOCAL = "ALLOW LOCAL";


		/// <summary>
		/// Системное имя CORS-профиля, активирующего поддержку передачи авторизационных данных 
		/// (Cookies, заголовки Authorization клиента) между различными доверенными доменами.
		/// </summary>
		public const string CORS_ALLOW_CREDENTIALS = "ALLOW CREDENTIALS";


		/// <summary>
		/// Системное имя CORS-профиля, разрешающего междоменные запросы исключительно 
		/// для безопасных HTTP-методов чтения данных (GET, HEAD, OPTIONS).
		/// </summary>
		public const string CORS_ALLOW_READ_ONLY = "ALLOW READ ONLY";


		/* methods */


		/// <summary>
		/// Выполняет пакетную инкапсулированную регистрацию всех пяти канонических политик CORS 
		/// в инфраструктуре ASP.NET Core на основе переданных параметров конфигурации библиотеки.
		/// </summary>
		/// <param name="corsOptions">Системные параметры конфигурации <see cref="Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions"/>.</param>
		/// <param name="options">Строго типизированные настройки глобальной конфигурации веб-библиотеки.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="corsOptions"/> или <paramref name="options"/> равен <see langword="null"/>.
		/// </exception>
		public static void RegisterPolicies(
			Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions corsOptions,
			LibWebOptions options)
		{
			ArgumentNullException.ThrowIfNull(corsOptions);
			ArgumentNullException.ThrowIfNull(options);

			// 1. ALLOW ALL: Полная свобода (подходит для открытых API и публичных сайтов)
			corsOptions.AddPolicy(CORS_ALLOW_ALL, policy =>
			{
				policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
			});

			// 2. ALLOW LOCAL: Разрешаем только локальные фронтенд-серверы отладки
			if (options.Cors?.LocalOrigins != null)
				corsOptions.AddPolicy(CORS_ALLOW_LOCAL, policy =>
				{
					policy.WithOrigins(options.Cors.LocalOrigins).AllowAnyHeader().AllowAnyMethod();
				});

			// 3. ALLOW TRUSTED: Строгий продакшн-режим для закрытых корпоративных контуров
			if (options.Cors?.TrustedOrigins != null)
				corsOptions.AddPolicy(CORS_ALLOW_TRUSTED, policy =>
				{
					policy.WithOrigins(options.Cors.TrustedOrigins).AllowAnyHeader().AllowAnyMethod();
				});

			// 4. ALLOW CREDENTIALS: Режим междоменной авторизации (АРМ) по кукам/сессиям фреймворка
			if (options.Cors?.LocalOrigins != null || options.Cors?.TrustedOrigins != null)
				corsOptions.AddPolicy(CORS_ALLOW_CREDENTIALS, policy =>
				{
					var origins1 = options.Cors.TrustedOrigins ?? options.Cors.LocalOrigins;
					policy.WithOrigins(origins1!).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
				});

			// 5. ALLOW READ ONLY: Режим безопасного чтения (только GET/HEAD запросы контента)
			corsOptions.AddPolicy(CORS_ALLOW_READ_ONLY, policy =>
			{
				policy.AllowAnyOrigin().AllowAnyHeader().WithMethods("GET", "HEAD", "OPTIONS");
			});
		}

	}

}
