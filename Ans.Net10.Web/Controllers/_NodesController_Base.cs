using Ans.Net10.Web.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace Ans.Net10.Web.Controllers
{

    /// <summary>
    /// Абстрактный базовый класс для контроллеров динамического рендеринга страниц.
    /// Предоставляет встроенный конвейер разрешения ЧПУ-путей через карту представлений.
    /// </summary>
    //[ApiExplorerSettings(IgnoreApi = true)]
    //[Route("/")]
    public abstract class _NodesController_Base
        : Controller
    {

        private readonly NodesMapper _mapper;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="_NodesController_Base"/>.
        /// </summary>
        /// <param name="mapper">Исполнитель карты путей к Razor-представлениям.</param>
        /// <exception cref="ArgumentNullException">Инициируется, если переданный маппер равен null.</exception>
        protected _NodesController_Base(
            NodesMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            _mapper = mapper;
        }


        /* actions */


        /// <summary>
        /// Перехватывает любые многосегментные HTTP-запросы и выполняет рендеринг соответствующего узла или страницы.
        /// </summary>
        /// <param name="path">Динамический относительный ЧПУ-путь из входящего URL-адреса. Допускает значение <see langword="null"/>.</param>
        /// <returns>
        /// Экземпляр <see cref="ViewResult"/> с физическим путем к .cshtml файлу в случае успеха, 
        /// либо <see cref="NotFoundResult"/> (HTTP 404), если страница отсутствует в карте.
        /// </returns>
        //[HttpGet("{*path}")]
        public virtual IActionResult Render(
            string? path)
        {
            string cleanPath1 = path is not null
                ? path.Trim('/')
                : string.Empty;
            if (_mapper.TryGetView(cleanPath1, out var viewPath1) && viewPath1 is not null)
                return View(viewPath1, model: cleanPath1);
            return NotFound();
        }

    }

}
