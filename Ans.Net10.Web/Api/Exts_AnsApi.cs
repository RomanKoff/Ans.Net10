// rev 2026-11-27

using Ans.Net10.Web.Compilation;
using Ans.Net10.Web.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using System.Diagnostics;

namespace Ans.Net10.Web.Api
{

    /// <summary>
    /// Представляет сводный результат пакетной проверки и прогрева компиляции всех Razor-представлений сайта.
    /// </summary>
    /// <param name="TotalViewsCount">Общее количество проверенных <c>.cshtml</c>-файлов.</param>
    /// <param name="SucceededCount">Количество успешно скомпилированных представлений.</param>
    /// <param name="FailedCount">Количество представлений, в которых обнаружены ошибки компиляции.</param>
    /// <param name="ElapsedMilliseconds">Общее время выполнения пакетной компиляции в миллисекундах.</param>
    /// <param name="Errors">Сводный список всех ошибок компиляции по всем проблемным файлам.</param>
    public sealed record AnsBatchViewsCompileResult(
        int TotalViewsCount,
        int SucceededCount,
        int FailedCount,
        long ElapsedMilliseconds,
        IReadOnlyList<AnsCompilationError> Errors);



    /// <summary>
    /// Представляет данные запроса на аутентификацию по персональному ключу системного доступа.
    /// </summary>
    /// <param name="Username">Имя пользователя из списка доступа.</param>
    /// <param name="Key">Персональный ключ доступа.</param>
    public sealed record AnsSystemAccessLoginRequest(
        string? Username,
        string? Key);



    /// <summary>
    /// Методы расширения для подключения диагностических и административных API-обработчиков библиотеки в конечном приложении.
    /// </summary>
    public static partial class Exts_AnsApi
    {

        /// <summary>
        /// Регистрирует конечные точки получения новых записей журнала (<c>/updates</c>) и очистки буфера (<c>/clear</c>),
        /// защищенные фильтром <see cref="AnsSystemAccessEndpointFilter"/>.
        /// </summary>
        /// <param name="endpoints">Построитель маршрутов приложения.</param>
        /// <param name="pattern">Базовый префикс маршрута API консоли (по умолчанию <c>"/api/ans/console"</c>).</param>
        /// <returns>Группа маршрутов <see cref="RouteGroupBuilder"/> для дополнительной настройки.</returns>
        public static RouteGroupBuilder AnsApi_Console(
            this IEndpointRouteBuilder endpoints,
            string pattern = "/api/ans/console")
        {
            var normalized1 = _normalizePattern(pattern, "/api/ans/console");
            var options1 = endpoints.ServiceProvider.GetService<LibWebOptions>();
            if (options1 is not null)
            {
                options1.SystemHelpers.ConsoleApiUrl = normalized1;
                if (!options1.Serilog.ExcludePaths.Contains(normalized1, StringComparer.OrdinalIgnoreCase))
                    options1.Serilog.ExcludePaths = [.. options1.Serilog.ExcludePaths, normalized1];
            }

            var group1 = endpoints.MapGroup(normalized1)
                .WithOrder(-100)
                .AddEndpointFilter<AnsSystemAccessEndpointFilter>();

            group1.MapGet("/updates", (
                AnsMemoryLogStore? logStore,
                long afterId = 0,
                LogEventLevel? minLevel = null) =>
            {
                if (logStore is null)
                    return Results.Ok(Array.Empty<AnsLogEntry>());
                return Results.Ok(logStore.GetEntries(afterId, minLevel));
            });

            group1.MapPost("/clear", (
                AnsMemoryLogStore? logStore) =>
            {
                logStore?.Clear();
                return Results.Ok(new { ok = true });
            });

            return group1;
        }


        /// <summary>
        /// Регистрирует конечные точки управления динамической компиляцией <c>App_Code</c>
        /// и диагностики Razor-представлений (<c>/reload</c>, <c>/batch</c>),
        /// защищенные фильтром <see cref="AnsSystemAccessEndpointFilter"/>.
        /// </summary>
        /// <param name="endpoints">Построитель маршрутов приложения.</param>
        /// <param name="pattern">Базовый префикс маршрута API компиляции (по умолчанию <c>"/api/ans/compile"</c>).</param>
        /// <returns>Группа маршрутов <see cref="RouteGroupBuilder"/> для дополнительной настройки.</returns>
        public static RouteGroupBuilder AnsApi_Compile(
            this IEndpointRouteBuilder endpoints,
            string pattern = "/api/ans/compile")
        {
            var normalized1 = _normalizePattern(pattern, "/api/ans/compile");
            var options1 = endpoints.ServiceProvider.GetService<LibWebOptions>();
            if (options1 is not null)
                options1.SystemHelpers.CompileApiUrl = normalized1;

            var group1 = endpoints.MapGroup(normalized1)
                .WithOrder(-100)
                .AddEndpointFilter<AnsSystemAccessEndpointFilter>();

            group1.MapPost("/reload", async (
                AnsAppCodeCompiler? appCodeCompiler,
                CancellationToken ct) =>
            {
                if (appCodeCompiler is null)
                    return Results.BadRequest(new { message = Resources.SystemHelpers.Error_DynamicCompilationDisabled });

                var res1 = await appCodeCompiler.CompileAsync(force: true, ct).ConfigureAwait(false);
                return Results.Ok(res1);
            });

            group1.MapPost("/batch", async (
                AnsRazorViewCompiler? viewCompiler,
                IWebHostEnvironment env,
                CancellationToken ct) =>
            {
                if (viewCompiler is null)
                    return Results.BadRequest(new { message = Resources.SystemHelpers.Error_DynamicCompilationDisabled });

                var sw1 = Stopwatch.StartNew();
                var paths1 = _collectAllViewPaths(env.ContentRootPath);
                var errors1 = new List<AnsCompilationError>();
                int succeeded1 = 0, failed1 = 0;

                foreach (var path1 in paths1)
                {
                    ct.ThrowIfCancellationRequested();
                    try
                    {
                        await viewCompiler.CompileAsync(path1).ConfigureAwait(false);
                        succeeded1++;
                    }
                    catch (AnsRazorCompilationException ex)
                    {
                        failed1++;
                        errors1.AddRange(ex.Errors);
                    }
                    catch (Exception ex)
                    {
                        failed1++;
                        errors1.Add(new AnsCompilationError(path1, 0, 0, "ERR", ex.Message));
                    }
                }

                sw1.Stop();
                return Results.Ok(new AnsBatchViewsCompileResult(
                    paths1.Count, succeeded1, failed1, sw1.ElapsedMilliseconds, errors1));
            });

            return group1;
        }


        /// <summary>
        /// Регистрирует конечные точки входа (<c>/login</c>) и выхода (<c>/logout</c>) 
        /// по персональному ключу системного доступа.
        /// </summary>
        /// <param name="endpoints">Построитель маршрутов приложения.</param>
        /// <param name="pattern">Базовый префикс маршрута API доступа (по умолчанию <c>"/api/ans/access"</c>).</param>
        /// <returns>Группа маршрутов <see cref="RouteGroupBuilder"/> для дополнительной настройки.</returns>
        public static RouteGroupBuilder AnsApi_SystemAccess(
            this IEndpointRouteBuilder endpoints,
            string pattern = "/api/ans/access")
        {
            var normalized1 = _normalizePattern(pattern, "/api/ans/access");
            var options1 = endpoints.ServiceProvider.GetService<LibWebOptions>();
            if (options1 is not null)
                options1.SystemHelpers.SystemAccessApiUrl = normalized1;

            var group1 = endpoints.MapGroup(normalized1).WithOrder(-100);

            group1.MapPost("/login", async (
                HttpContext ctx,
                IAntiforgery antiforgery,
                AnsSystemAccessService accessService,
                [FromBody] AnsSystemAccessLoginRequest request) =>
            {
                if (!accessService.HasAccessList)
                    return Results.NotFound();

                try
                {
                    await antiforgery.ValidateRequestAsync(ctx).ConfigureAwait(false);
                }
                catch (AntiforgeryValidationException)
                {
                    return Results.BadRequest(new { message = Resources.SystemHelpers.Access_ErrorInvalidToken });
                }

                var result1 = await accessService
                    .TryLoginAsync(request.Username, request.Key)
                    .ConfigureAwait(false);

                return result1 switch
                {
                    SystemAccessLoginResult.Success => Results.Ok(new { ok = true }),
                    SystemAccessLoginResult.TooManyAttempts => Results.StatusCode(StatusCodes.Status429TooManyRequests),
                    _ => Results.Unauthorized()
                };
            });

            group1.MapPost("/logout", async (
                HttpContext ctx,
                IAntiforgery antiforgery,
                AnsSystemAccessService accessService) =>
            {
                try
                {
                    await antiforgery.ValidateRequestAsync(ctx).ConfigureAwait(false);
                }
                catch (AntiforgeryValidationException)
                {
                    return Results.BadRequest(new { message = Resources.SystemHelpers.Access_ErrorInvalidToken });
                }

                accessService.Logout();
                return Results.Ok(new { ok = true });
            });

            return group1;
        }


        /* privates */


        private static string _normalizePattern(
            string? pattern,
            string defaultPattern)
        {
            return string.IsNullOrWhiteSpace(pattern)
                ? defaultPattern
                : $"/{pattern.Trim().Trim('/')}";
        }


        private static List<string> _collectAllViewPaths(
            string contentRootPath)
        {
            var result1 = new List<string>();
            _scanViewsDirectory(contentRootPath, "Views", result1);
            _scanViewsDirectory(contentRootPath, "Pages", result1);
            result1.Sort(StringComparer.OrdinalIgnoreCase);
            return result1;
        }


        private static void _scanViewsDirectory(
            string contentRootPath,
            string folderName,
            List<string> targetList)
        {
            var fullDir1 = Path.Combine(contentRootPath, folderName);
            if (!Directory.Exists(fullDir1))
                return;

            foreach (var file1 in Directory.EnumerateFiles(fullDir1, "*.cshtml", SearchOption.AllDirectories))
            {
                var name1 = Path.GetFileName(file1);
                if (name1.StartsWith('_') || name1.Contains('~'))
                    continue;

                var relPath1 = Path.GetRelativePath(contentRootPath, file1).Replace('\\', '/');
                targetList.Add($"/{relPath1}");
            }
        }

    }

}
