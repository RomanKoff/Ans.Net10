// rev 2026-11-12

using Ans.Net10.Common;
using Ans.Net10.Common.Services;
using Ans.Net10.Web.Middlewares;
using Ans.Net10.Web.Nodes;
using Ans.Net10.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace Ans.Net10.Web
{

    /// <summary>
    /// Предоставляет статические методы расширения для автоматической "zero-configuration" 
    /// инициализации и настройки инфраструктуры веб-библиотеки Ans в конвейере приложения.
    /// </summary>
    public static partial class LibWebStartup
    {

        /* functions */


        /// <summary>
        /// Выполняет централизованную регистрацию сервисов библиотеки.
        /// </summary>
        /// <param name="builder">Архитектурный строитель веб-приложения <see cref="WebApplicationBuilder"/>.</param>
        /// <returns>Строитель <see cref="WebApplicationBuilder "/> для последующей кастомизации конвейера MVC фреймворка.</returns>
        /// <exception cref="ArgumentNullException">
        /// Вызывается, если переданный параметр <paramref name="builder"/> равен <see langword="null"/>.
        /// </exception>
        public static WebApplicationBuilder Add_AnsNet10Web(
            this WebApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            SuppCulture.AddCodePagesSupport();

            var options1 = AppSettingsFactory.GetOptions<LibWebOptions>(builder.Configuration);
            builder.Services.AddSingleton(options1);

            var env1 = builder.Environment;
            var appName1 = options1.AppName ?? env1.ApplicationName;

            builder.Services.Configure<RequestLocalizationOptions>(o =>
            {
                var defaultCulture1 = new CultureInfo(options1.DefaultCulture);
                o.DefaultRequestCulture = new(defaultCulture1);
                o.SupportedCultures = [defaultCulture1];
                o.SupportedUICultures = [defaultCulture1];
            });

            if (options1.Serilog.Enabled)
            {
                const string logTemplate1 = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{Exception}\n";
                static DateTime funcLogHourKey1(LogEvent x) =>
                    new(x.Timestamp.Year, x.Timestamp.Month, x.Timestamp.Day, x.Timestamp.Hour, 0, 0, DateTimeKind.Utc);
                var basePath1 = string.IsNullOrWhiteSpace(options1.Serilog.Path)
                    ? "logs" : options1.Serilog.Path.TrimEnd('/');
                if (!Enum.TryParse<LogEventLevel>(options1.Serilog.MinimumLevel, true, out var rootLevel1))
                    rootLevel1 = LogEventLevel.Information;
                builder.Host.UseSerilog((context, services, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .MinimumLevel.Is(rootLevel1)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()

                        // Debug
                        .WriteTo.Logger(lc => lc
                            .Filter.ByIncludingOnly(x => x.Level == LogEventLevel.Debug)
                            .WriteTo.Map(
                                keySelector: funcLogHourKey1,
                                configure: (dateTime1, writer1) => writer1.File(
                                    path: $"{basePath1}/{appName1}/{dateTime1:yyyy-MM}/debug_{dateTime1:dd-HH}.txt",
                                    rollingInterval: RollingInterval.Infinite,
                                    retainedFileCountLimit: null,
                                    outputTemplate: logTemplate1
                                )))

                        // Information
                        .WriteTo.Logger(lc => lc
                            .Filter.ByIncludingOnly(x => x.Level == LogEventLevel.Information)
                            .WriteTo.Map(
                                keySelector: funcLogHourKey1,
                                configure: (dateTime1, writer1) => writer1.File(
                                    path: $"{basePath1}/{appName1}/{dateTime1:yyyy-MM}/info_{dateTime1:dd-HH}.txt",
                                    rollingInterval: RollingInterval.Infinite,
                                    retainedFileCountLimit: null,
                                    outputTemplate: logTemplate1
                                )))

                        // Warning
                        .WriteTo.Logger(lc => lc
                            .Filter.ByIncludingOnly(x => x.Level == LogEventLevel.Warning)
                            .WriteTo.Map(
                                keySelector: funcLogHourKey1,
                                configure: (dateTime1, writer1) => writer1.File(
                                    path: $"{basePath1}/{appName1}/{dateTime1:yyyy-MM}/warn_{dateTime1:dd-HH}.txt",
                                    rollingInterval: RollingInterval.Infinite,
                                    retainedFileCountLimit: null,
                                    outputTemplate: logTemplate1
                                )))

                        // Error
                        .WriteTo.Logger(lc => lc
                            .Filter.ByIncludingOnly(x => x.Level == LogEventLevel.Error)
                            .WriteTo.Map(
                                keySelector: funcLogHourKey1,
                                configure: (dateTime1, writer1) => writer1.File(
                                    path: $"{basePath1}/{appName1}/{dateTime1:yyyy-MM}/err_{dateTime1:dd-HH}.txt",
                                    rollingInterval: RollingInterval.Infinite,
                                    retainedFileCountLimit: null,
                                    outputTemplate: logTemplate1
                                )))

                        // Fatal
                        .WriteTo.Logger(lc => lc
                            .MinimumLevel.Fatal()
                            .WriteTo.Map(
                                keySelector: funcLogHourKey1,
                                configure: (dateTime1, writer1) => writer1.File(
                                    path: $"{basePath1}/{appName1}/{dateTime1:yyyy-MM}/fatal_{dateTime1:dd-HH}.txt",
                                    rollingInterval: RollingInterval.Infinite,
                                    retainedFileCountLimit: null,
                                    outputTemplate: logTemplate1
                                )));
                });
            }

            var dpBasePath1 = string.IsNullOrWhiteSpace(options1.DataProtection.Path)
                ? "data-protection"
                : options1.DataProtection.Path.TrimEnd('/');
            var finalDpPath1 = Path.Combine(dpBasePath1, appName1);
            var dpBuilder1 = builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(finalDpPath1))
                .SetApplicationName(appName1);
            if (!string.IsNullOrWhiteSpace(options1.DataProtection.CertificatePath))
            {
                var certPath1 = Path.Combine(builder.Environment.ContentRootPath, options1.DataProtection.CertificatePath);
                if (File.Exists(certPath1))
                {
                    using var certificate1 = X509CertificateLoader.LoadPkcs12FromFile(
                        certPath1,
                        options1.DataProtection.CertificatePassword,
                        X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.EphemeralKeySet);
                    dpBuilder1.ProtectKeysWithCertificate(certificate1);
                }
            }

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHybridCache(o =>
            {
                o.DefaultEntryOptions = SuppCache.HYBRID_CACHE_5MIN;
            });
            builder.Services.AddHttpClient();
            builder.Services.AddResponseCaching();
            builder.Services.AddExceptionHandler<Ans_ExceptionHandler>();

            builder.Services.AddCors(o => SuppCors.RegisterPolicies(o, options1));

            if (options1.UseSession)
            {
                builder.Services.AddDistributedMemoryCache();
                builder.Services.AddSession();
            }

            //--- Nodes mapping setup
            if (options1.UseNodes)
            {
                builder.Services.AddSingleton<NodesMapper>();
                builder.Services.AddHostedService<NodesWatcherService>();
            }

            //--- XmlSitemap
            builder.Services.AddSingleton<SitemapBuilder>();
            if (options1.XmlSitemap is not null)
                builder.Services.AddHostedService<SitemapWatcherService>();

            //--- mail MailService
            if (options1.MailService is not null)
                builder.Services.AddSingleton<IMailerService, Ans_MailerService>(
                    _ => new Ans_MailerService(options1.MailService));
            else
                builder.Services.AddSingleton<IMailerService, Fake_MailerService>();

            //--- PuppeteerSharpService
            if (options1.PuppeteerSharpService is not null)
                builder.Services.AddSingleton<IBrowserFactory, PuppeteerBrowserFactory>(
                    _ => new PuppeteerBrowserFactory(options1.PuppeteerSharpService));

            builder.Services.AddScoped<IViewRenderService, Ans_ViewRenderService>();
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

            SuppConsole.WriteColor(ConsoleColor.Yellow, "\nAdd_AnsNet10Web — OK");
            SuppConsole.WriteColor(ConsoleColor.Yellow, "\n======================================\n\n");

            return builder;
        }


        /* methods */


        /// <summary>
        /// Асинхронно конфигурирует конвейер обработки входящих HTTP-запросов (Middleware Pipeline).
        /// </summary>
        /// <param name="app">Экземпляр выполняемого веб-приложения <see cref="WebApplication"/>.</param>
        /// <param name="sitemapBuilder">Опциональный делегат для программной настройки или дополнения карты сайта.</param>
        /// <returns>Задача, представляющая асинхронную операцию настройки конвейера.</returns>
        /// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="app"/> равен <see langword="null"/>.</exception>
        public static async Task Use_AnsNet10WebAsync(
            this WebApplication app,
            Action<SitemapBuilder>? sitemapBuilder = null)
        {
            ArgumentNullException.ThrowIfNull(app);

            var options1 = app.Services.GetRequiredService<LibWebOptions>();

            app.UseRequestLocalization();

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

            //--- proxy
            if (options1.Proxy is not null && options1.Proxy.UseForwardedHeaders)
            {
                var forwardedOptions1 = new ForwardedHeadersOptions();
                SuppForwardedHeaders.Configure(forwardedOptions1, options1);
                app.UseForwardedHeaders(forwardedOptions1);
            }

            //--- fix url
            app.UseMiddleware<AnsUrlCanonicalizationMiddleware>();

            //--- sitemap profiler
            if (options1.XmlSitemap is not null)
                app.UseMiddleware<AnsSitemapProfilerMiddleware>();

            app.UseResponseCaching();

            //--- sessions
            if (options1.UseSession)
                app.UseSession();

            //--- static files
            if (!options1.UseRuntimeCompilation)
                app.MapStaticAssets();
            if (options1.Mimetypes is not null && options1.Mimetypes.Length > 0)
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
                app.UseStaticFiles(new StaticFileOptions
                {
                    ContentTypeProvider = mimeProvider1
                });
            }
            else
                app.UseStaticFiles();

            app.UseRouting();

            //--- cors
            if (string.IsNullOrWhiteSpace(options1.Cors?.Profile))
                app.UseCors(SuppCors.CORS_ALLOW_ALL);
            else
                app.UseCors(options1.Cors.Profile);

            if (options1.Routes != null)
                app.AddRoutes(options1.Routes);

            app.MapControllers();
            app.MapRazorPages();

            //--- sitemap initialization
            if (options1.XmlSitemap is not null || sitemapBuilder is not null)
            {
                var sitemapBuilder1 = app.Services.GetRequiredService<SitemapBuilder>();
                sitemapBuilder1.CustomBuilder = sitemapBuilder;
                await sitemapBuilder1.InitializeAsync(app.Lifetime.ApplicationStarted)
                    .ConfigureAwait(false);
            }

            //--- nodes views initialization
            if (options1.UseNodes)
            {
                var nodesMapper1 = app.Services.GetRequiredService<NodesMapper>();
                await nodesMapper1.InitializeAsync(app.Lifetime.ApplicationStarted)
                    .ConfigureAwait(false);
            }

            SuppConsole.WriteColor(ConsoleColor.Yellow, "\nUse_AnsNet10WebAsync — OK");
            SuppConsole.WriteColor(ConsoleColor.Yellow, "\n======================================\n\n");


            // Принудительно очищаем ресурсы при остановке веб-сервера
            var lifetime1 = app.Services.GetRequiredService<IHostApplicationLifetime>();
            lifetime1.ApplicationStopping.Register(() =>
            {
                if (app.Services.GetService<IBrowserFactory>() is IAsyncDisposable factory1)
                    Task.Run(async () => { await factory1.DisposeAsync(); }).GetAwaiter().GetResult();
            });
        }

    }

}
