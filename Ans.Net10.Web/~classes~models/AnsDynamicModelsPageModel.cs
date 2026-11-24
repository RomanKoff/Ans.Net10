// rev 2026-11-23

using Ans.Net10.Web.Compilation;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ans.Net10.Web
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
    /// Базовая модель системной страницы администратора для управления пересборкой <c>App_Code</c> 
    /// и диагностики рантайм-компиляции Razor-представлений.
    /// </summary>
    /// <param name="current">Текущий оркестровый контекст обработки запроса.</param>
    /// <param name="appCodeCompiler">Опциональный компилятор исходников <c>App_Code</c> (если включен режим <c>DynamicCompilation</c>).</param>
    /// <param name="viewCompiler">Опциональный рантайм-компилятор Razor-представлений (если включен режим <c>DynamicCompilation</c>).</param>
    public class AnsDynamicModelsPageModel(
        CurrentContext current,
        AnsAppCodeCompiler? appCodeCompiler = null,
        AnsRazorViewCompiler? viewCompiler = null)
        : AnsPageSystemModel(current)
    {

        /* readonly properties */


        /// <summary>
        /// Возвращает признак того, что подсистема динамической компиляции <c>DynamicCompilation</c> активна и настроена.
        /// </summary>
        public bool IsDynamicCompilationEnabled
            => appCodeCompiler is not null && viewCompiler is not null;


        /* properties */


        /// <summary>
        /// Получает или задает виртуальный путь к <c>.cshtml</c>-файлу для точечной тестовой проверки компиляции.
        /// </summary>
        [BindProperty]
        public string? TestViewPath { get; set; }


        /// <summary>
        /// Возвращает результат последней выполненной компиляции каталога <c>App_Code</c>.
        /// </summary>
        public AnsAppCodeCompileResult? CompileResult { get; private set; }
            = appCodeCompiler?.LastResult;


        /// <summary>
        /// Возвращает признак успешности точечной тестовой компиляции представления.
        /// </summary>
        public bool? TestViewSuccess { get; private set; }


        /// <summary>
        /// Возвращает список ошибок компиляции, зафиксированных при точечной проверке представления.
        /// </summary>
        public IReadOnlyList<AnsCompilationError> TestViewErrors { get; private set; } = [];


        /// <summary>
        /// Возвращает сводный отчет пакетной проверки компиляции всех представлений сайта.
        /// </summary>
        public AnsBatchViewsCompileResult? BatchResult { get; private set; }


        /* functions */


        /// <summary>
        /// Выполняет принудительную асинхронную пересборку всех <c>.cs</c>-файлов каталога <c>App_Code</c> 
        /// и сбрасывает кэш скомпилированных представлений.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
        /// <returns>Результат выполнения действия страницы <see cref="IActionResult"/>.</returns>
        public async Task<IActionResult> OnPostReloadAsync(
            CancellationToken cancellationToken)
        {
            var authCheck1 = base.OnGet();
            if (authCheck1 is NotFoundResult)
                return authCheck1;

            if (appCodeCompiler is null)
                return Page();

            CompileResult = await appCodeCompiler
                .CompileAsync(force: true, cancellationToken)
                .ConfigureAwait(false);

            return Page();
        }

        /// <summary>
        /// Выполняет принудительную тестовую компиляцию указанного Razor-представления 
        /// для проверки совместимости с текущими типами <c>App_Code</c>.
        /// </summary>
        /// <returns>Результат выполнения действия страницы <see cref="IActionResult"/>.</returns>
        public async Task<IActionResult> OnPostCompileViewAsync()
        {
            var authCheck1 = base.OnGet();
            if (authCheck1 is NotFoundResult)
                return authCheck1;

            if (viewCompiler is null || string.IsNullOrWhiteSpace(TestViewPath))
                return Page();

            try
            {
                await viewCompiler
                    .CompileAsync(TestViewPath)
                    .ConfigureAwait(false);
                TestViewSuccess = true;
                TestViewErrors = [];
            }
            catch (AnsRazorCompilationException ex)
            {
                TestViewSuccess = false;
                TestViewErrors = ex.Errors;
            }
            catch (Exception ex)
            {
                TestViewSuccess = false;
                TestViewErrors = [new AnsCompilationError(TestViewPath, 0, 0, "ERR", ex.Message)];
            }

            return Page();
        }


        /// <summary>
        /// Выполняет пакетную компиляцию и прогрев кэша всех Razor-представлений (<c>.cshtml</c>), 
        /// обнаруженных в каталогах <c>Views</c> и <c>Pages</c> веб-приложения.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
        /// <returns>Результат выполнения действия страницы <see cref="IActionResult"/>.</returns>
        public async Task<IActionResult> OnPostCompileAllViewsAsync(
            CancellationToken cancellationToken)
        {
            var authCheck1 = base.OnGet();
            if (authCheck1 is NotFoundResult)
                return authCheck1;

            if (viewCompiler is null)
                return Page();

            var stopwatch1 = Stopwatch.StartNew();
            var viewPaths1 = _collectAllViewPaths();
            var allErrors1 = new List<AnsCompilationError>();
            var succeeded1 = 0;
            var failed1 = 0;

            foreach (var path1 in viewPaths1)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    await viewCompiler
                        .CompileAsync(path1)
                        .ConfigureAwait(false);
                    succeeded1++;
                }
                catch (AnsRazorCompilationException ex)
                {
                    failed1++;
                    allErrors1.AddRange(ex.Errors);
                }
                catch (Exception ex)
                {
                    failed1++;
                    allErrors1.Add(new AnsCompilationError(path1, 0, 0, "ERR", ex.Message));
                }
            }

            stopwatch1.Stop();
            BatchResult = new AnsBatchViewsCompileResult(
                TotalViewsCount: viewPaths1.Count,
                SucceededCount: succeeded1,
                FailedCount: failed1,
                ElapsedMilliseconds: stopwatch1.ElapsedMilliseconds,
                Errors: allErrors1);

            return Page();
        }

        /* privates */


        private List<string> _collectAllViewPaths()
        {
            var result1 = new List<string>();
            var rootPath1 = Current.Env.ContentRootPath;

            _scanViewsDirectory(rootPath1, "Views", result1);
            _scanViewsDirectory(rootPath1, "Pages", result1);

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
                var fileName1 = Path.GetFileName(file1);
                if (fileName1.Equals("_ViewStart.cshtml", StringComparison.OrdinalIgnoreCase)
                    || fileName1.Equals("_ViewImports.cshtml", StringComparison.OrdinalIgnoreCase))
                    continue;

                var relPath1 = Path.GetRelativePath(contentRootPath, file1).Replace('\\', '/');
                targetList.Add($"/{relPath1}");
            }
        }

    }

}
