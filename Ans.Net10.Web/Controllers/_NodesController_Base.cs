// rev 2026-11-18

using Ans.Net10.Web.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace Ans.Net10.Web.Controllers
{

    /// <summary>
    /// Абстрактный базовый класс для контроллеров динамического рендеринга страниц.
    /// Предоставляет встроенный конвейер разрешения ЧПУ-путей через карту представлений.
    /// </summary>
    public abstract class _NodesController_Base
        : Controller
    {

        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="_NodesController_Base"/>.
        /// </summary>
        /// <param name="mapper">Исполнитель карты путей к Razor-представлениям.</param>
        /// <exception cref="ArgumentNullException">Инициируется, если переданный маппер равен <see langword="null"/>.</exception>
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[Route("/")]
        protected _NodesController_Base(
            NodesMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            Mapper = mapper;
        }


        /* readonly properties */


        /// <summary>
        /// Получает исполнитель карты путей к Razor-представлениям динамических узлов.
        /// </summary>
        /// <returns>Экземпляр <see cref="NodesMapper"/>.</returns>
        protected NodesMapper Mapper { get; }


        /* actions */


        /// <summary>
        /// Перехватывает многосегментные HTTP-запросы и выполняет рендеринг соответствующего узла или страницы.
        /// </summary>
        /// <param name="path">Динамический относительный ЧПУ-путь из входящего URL-адреса. Допускает значение <see langword="null"/>.</param>
        /// <returns>
        /// Экземпляр <see cref="ViewResult"/> с виртуальным путем к <c>.cshtml</c> файлу в случае успеха, 
        /// либо <see cref="NotFoundResult"/> (HTTP 404), если страница отсутствует в карте.
        /// </returns>
        //[HttpGet("{*path}")]
        public virtual IActionResult Render(
            string? path)
        {
            string cleanPath1 = path is not null
                ? path.Trim('/')
                : string.Empty;
            if (Mapper.TryGetView(cleanPath1, out string? viewPath1) && viewPath1 is not null)
                return View(viewPath1, model: cleanPath1);
            return NotFound();
        }

    }

}
