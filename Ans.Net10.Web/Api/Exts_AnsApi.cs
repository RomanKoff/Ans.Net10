// rev 2026-11-25

using Ans.Net10.Web.Compilation;
using Ans.Net10.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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
    /// Методы расширения для подключения диагностических и административных API-обработчиков библиотеки в конечном приложении.
    /// </summary>
    public static partial class Exts_AnsApi
    {

        /// <summary>
        /// Регистрирует конечные точки получения новых записей журнала (<c>/updates</c>) и очистки буфера (<c>/clear</c>).
        /// </summary>
        /// <param name="endpoints">Построитель маршрутов приложения.</param>
        /// <param name="pattern">Базовый префикс маршрута API консоли (по умолчанию <c>"/api/ans/console"</c>).</param>
        /// <returns>Группа маршрутов <see cref="RouteGroupBuilder"/> для настройки авторизации.</returns>
        public static RouteGroupBuilder AnsApi_Console(
            this IEndpointRouteBuilder endpoints,
            string pattern = "/api/ans/console")
        {
            var group1 = endpoints._createApiGroup(pattern, "/api/ans/console");

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
        /// и диагностики Razor-представлений (<c>/reload</c>, <c>/batch</c>, <c>/view</c>).
        /// </summary>
        /// <param name="endpoints">Построитель маршрутов приложения.</param>
        /// <param name="pattern">Базовый префикс маршрута API компиляции (по умолчанию <c>"/api/ans/compile"</c>).</param>
        /// <returns>Группа маршрутов <see cref="RouteGroupBuilder"/> для настройки авторизации.</returns>
        public static RouteGroupBuilder AnsApi_Compile(
            this IEndpointRouteBuilder endpoints,
            string pattern = "/api/ans/compile")
        {
            var group1 = endpoints._createApiGroup(pattern, "/api/ans/compile");

            group1.MapPost("/reload", async (
                AnsAppCodeCompiler? appCodeCompiler,
                CancellationToken ct) =>
            {
                if (appCodeCompiler is null)
                    return Results.BadRequest(new { message = "DynamicCompilation отключен." });

                var res1 = await appCodeCompiler.CompileAsync(force: true, ct).ConfigureAwait(false);
                return Results.Ok(res1);
            });

            group1.MapPost("/batch", async (
                AnsRazorViewCompiler? viewCompiler,
                IWebHostEnvironment env,
                CancellationToken ct) =>
            {
                if (viewCompiler is null)
                    return Results.BadRequest(new { message = "DynamicCompilation отключен." });

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


        /* privates */


        private static RouteGroupBuilder _createApiGroup(
            this IEndpointRouteBuilder endpoints,
            string? pattern,
            string defaultPattern)
        {
            var normalized1 = string.IsNullOrWhiteSpace(pattern)
                ? defaultPattern
                : $"/{pattern.Trim().Trim('/')}";
            return endpoints.MapGroup(normalized1).WithOrder(-100);
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
                if (name1.Equals("_ViewStart.cshtml", StringComparison.OrdinalIgnoreCase)
                    || name1.Equals("_ViewImports.cshtml", StringComparison.OrdinalIgnoreCase))
                    continue;

                var relPath1 = Path.GetRelativePath(contentRootPath, file1).Replace('\\', '/');
                targetList.Add($"/{relPath1}");
            }
        }

    }

}