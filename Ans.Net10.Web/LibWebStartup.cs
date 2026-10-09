// rev 2026-10-09

using Ans.Net10.Common;
using Ans.Net10.Common.Services;
using Ans.Net10.Web.Middlewares;
using Ans.Net10.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Предоставляет статические методы расширения для автоматической "zero-configuration" 
	/// инициализации и настройки инфраструктуры веб-библиотеки Ans в конвейере приложения.
	/// </summary>
	public static class LibWebStartup
	{

		/// <summary>
		/// Выполняет централизованную регистрацию сервисов библиотеки.
		/// </summary>
		/// <param name="builder">Архитектурный строитель веб-приложения <see cref="WebApplicationBuilder"/>.</param>
		/// <returns>Строитель <see cref="IMvcBuilder"/> для последующей кастомизации конвейера MVC фреймворка.</returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если переданный параметр <paramref name="builder"/> равен <see langword="null"/>.
		/// </exception>
		public static IMvcBuilder Add_AnsNet10Web(
			this WebApplicationBuilder builder)
		{
			ArgumentNullException.ThrowIfNull(builder);

			SuppCulture.AddCodePagesSupport();

			var options1 = AppSettingsFactory.GetOptions<LibWebOptions>(builder.Configuration);
			builder.Services.AddSingleton(options1);

			var defaultCultureName1 = options1.DefaultCulture;
			builder.Services.Configure<RequestLocalizationOptions>(o =>
			{
				var defaultCulture1 = new CultureInfo(defaultCultureName1);
				o.DefaultRequestCulture = new(defaultCulture1);
				o.SupportedCultures = [defaultCulture1];
				o.SupportedUICultures = [defaultCulture1];
			});

			builder.Services.AddHttpContextAccessor();
			builder.Services.AddHybridCache(o =>
			{
				o.DefaultEntryOptions = SuppCache.HYBRID_CACHE_5MIN;
			});
			builder.Services.AddHttpClient();
			builder.Services.AddResponseCaching();
			builder.Services.AddExceptionHandler<AnsExceptionHandler>();

			builder.Services.AddCors(o => SuppCors.RegisterPolicies(o, options1));

			if (options1.UseSession)
			{
				builder.Services.AddDistributedMemoryCache();
				builder.Services.AddSession();
			}

			//builder.Services.AddSingleton<Nodes.INodePathResolver, Nodes.AnsNodePathResolver>();
			//builder.Services.AddSingleton<ISiteProfileProvider, AnsXmlSiteProfileProvider>();

			if (options1.MailService == null)
				builder.Services.AddSingleton<IMailerService, FakeMailerService>();
			else
				builder.Services.AddSingleton<IMailerService, AnsMailerService>(
					_ => new AnsMailerService(options1.MailService));

			builder.Services.AddScoped<IViewRenderService, AnsViewRenderService>();
			//builder.Services.AddScoped<ICmsProfileResolver, AnsCmsProfileResolver>();
			builder.Services.AddScoped<CurrentContext>();

			builder.Services.AddRazorPages();

			var mvcBuilder1 = builder.Services
				.AddControllersWithViews(o =>
				{
					o.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
						_ => Common.Resources.Form.Text_ValueIsRequired);
					foreach (var profile1 in SuppWebCache.HTTP_CACHE_PROFILES)
						o.CacheProfiles.Add(profile1.Key, profile1.Value);
					if (options1.UseSession)
						o.Filters.Add(new ResponseCacheAttribute
						{
							CacheProfileName = SuppWebCache.HTTP_CACHE_NONE_NAME
						});
				});

			mvcBuilder1.AddJsonOptions(o =>
			{
				var baseOptions1 = SuppJson.DEFAULT_JSON_SERIALIZER_OPTIONS;
				o.JsonSerializerOptions.DefaultIgnoreCondition = baseOptions1.DefaultIgnoreCondition;
				o.JsonSerializerOptions.PropertyNameCaseInsensitive = baseOptions1.PropertyNameCaseInsensitive;
				o.JsonSerializerOptions.WriteIndented = baseOptions1.WriteIndented;
				o.JsonSerializerOptions.Encoder = baseOptions1.Encoder;
			});

			if (options1.UseRuntimeCompilation)
				mvcBuilder1.AddRazorRuntimeCompilation();

			return mvcBuilder1;
		}


		/// <summary>
		/// Асинхронно конфигурирует конвейер обработки входящих HTTP-запросов (Middleware Pipeline).
		/// </summary>
		/// <param name="app">Экземпляр выполняемого веб-приложения <see cref="WebApplication"/>.</param>
		/// <returns>Задача, представляющая асинхронную операцию настройки конвейера.</returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если переданный параметр <paramref name="app"/> равен <see langword="null"/>.
		/// </exception>
		public static async Task Use_AnsNet10WebAsync(
			this WebApplication app)
		{
			ArgumentNullException.ThrowIfNull(app);

			var options1 = app.Services.GetRequiredService<LibWebOptions>();

			if (options1.UseDeveloperMode)
			{
				app.UseDeveloperExceptionPage();
				app.UseStatusCodePages();
			}
			else
			{
				var path1 = options1.Errors.RazorPageErrorsPath;
				app.UseExceptionHandler(path1);
				app.UseStatusCodePagesWithReExecute(path1, "?code={0}");
			}

			if (!options1.UseDeveloperMode)
				app.UseHsts();
			app.UseHttpsRedirection();

			if (options1.Proxy != null && options1.Proxy.UseForwardedHeaders)
			{
				var forwardedOptions1 = new ForwardedHeadersOptions();
				SuppForwardedHeaders.Configure(forwardedOptions1, options1);
				app.UseForwardedHeaders(forwardedOptions1);
			}

			app.UseMiddleware<AnsTrailingSlashMiddleware>();

			app.UseResponseCaching();
			app.UseRequestLocalization();

			if (options1.UseSession)
				app.UseSession();

			if (options1.Mimetypes != null && options1.Mimetypes.Length > 0)
			{
				var mimeProvider1 = new FileExtensionContentTypeProvider();
				foreach (var mime1 in options1.Mimetypes)
				{
					if (string.IsNullOrEmpty(mime1))
						continue;
					var parts1 = mime1.Split("|");
					if (parts1.Length == 2
						&& !string.IsNullOrWhiteSpace(parts1[0])
						&& !string.IsNullOrWhiteSpace(parts1[1]))
						mimeProvider1.Mappings[parts1[0]] = parts1[1];
				}
				app.UseStaticFiles(
					new StaticFileOptions { ContentTypeProvider = mimeProvider1 });
			}
			else
				app.UseStaticFiles();

			app.UseRouting();

			//app.UseMiddleware<AnsCmsProfileMiddleware>();

			if (string.IsNullOrWhiteSpace(options1.Cors?.Profile))
				app.UseCors(SuppCors.CORS_ALLOW_ALL);
			else
				app.UseCors(options1.Cors.Profile);

			if (options1.Routes != null)
				app.AddRoutes(options1.Routes);

			app.MapControllers();
			app.MapRazorPages();

			//using var scope = app.Services.CreateScope();
			//var resolver = scope.ServiceProvider.GetRequiredService<INodePathResolver>();
			//await resolver.InitializeAsync(app.Lifetime.ApplicationStopping);
		}

	}

}
