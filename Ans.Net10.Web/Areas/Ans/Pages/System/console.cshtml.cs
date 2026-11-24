using Ans.Net10.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog.Events;

namespace Ans.Net10.Web.Areas.Ans.Pages.System
{

    /// <summary>
    /// Модель технической страницы просмотра системного журнала в реальном времени.
    /// </summary>
    public sealed class ConsoleModel(
        LibWebOptions options,
        AnsMemoryLogStore? logStore = null)
        : PageModel
    {

        /// <summary>
        /// Возвращает признак того, что буферизация логов в памяти активна.
        /// </summary>
        public bool IsEnabled
            => logStore is not null;


        /// <summary>
        /// Возвращает максимальную емкость буфера логов.
        /// </summary>
        public int Capacity
            => logStore?.Capacity ?? 0;


        /// <summary>
        /// Начальный снимок записей лога при открытии страницы.
        /// </summary>
        public AnsLogEntry[] Entries { get; private set; } = [];


        /// <summary>
        /// Обрабатывает первичный GET-запрос страницы.
        /// </summary>
        public IActionResult OnGet()
        {
            if (!_hasAccess())
                return Forbid();

            if (logStore is not null)
                Entries = logStore.GetEntries();

            return Page();
        }


        /// <summary>
        /// Возвращает новые записи лога в формате JSON для живого обновления терминала.
        /// </summary>
        public IActionResult OnGetUpdates(
            long afterId = 0,
            LogEventLevel? minLevel = null)
        {
            if (!_hasAccess())
                return Forbid();

            if (logStore is null)
                return new JsonResult(Array.Empty<AnsLogEntry>());

            var updates1 = logStore.GetEntries(afterId, minLevel);
            return new JsonResult(updates1);
        }


        /// <summary>
        /// Очищает кольцевой буфер логов в памяти.
        /// </summary>
        public IActionResult OnPostClear()
        {
            if (!_hasAccess())
                return Forbid();

            logStore?.Clear();
            return new JsonResult(new { ok = true });
        }


        /* privates */


        private bool _hasAccess()
        {
            if (options.UseDeveloperMode)
                return true;

            //var remoteIp1 = HttpContext.Connection.RemoteIpAddress;
            //if (remoteIp1 is not null
            //    && options.Subnets?.AdminSubnets is { } adminSubnets1
            //    && adminSubnets1.Contains(remoteIp1))
            //    return true;

            //if (!string.IsNullOrWhiteSpace(options.SystemToken))
            //{
            //    var token1 = Request.Query["token"].FirstOrDefault()
            //        ?? Request.Headers["X-System-Token"].FirstOrDefault();

            //    if (string.Equals(token1, options.SystemToken, StringComparison.Ordinal))
            //        return true;
            //}

            //return false;

            return true;
        }

    }

}
