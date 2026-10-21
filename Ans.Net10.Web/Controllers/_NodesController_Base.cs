using Ans.Net10.Web.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ans.Net10.Web.Controllers
{

	/// <summary>
	/// Базовый класс централизованного CMS-контроллера, обеспечивающий перехват динамических ЧПУ-маршрутов, 
	/// их разрешение через внутренний реестр представлений и рендеринг контента.
	/// </summary>
	//[ApiExplorerSettings(IgnoreApi = true)]
	//[Route("/")]
	public class _NodesController_Base
		: Controller
	{

		private readonly NodesResolver _resolver;
		private readonly ILogger<_NodesController_Base> _logger;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="_NodesController_Base"/>.
		/// </summary>
		/// <param name="resolver">Служба анализа и разрешения путей CMS в памяти.</param>
		/// <param name="logger">Служба ведения системных логов.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если один из входящих параметров равен <see langword="null"/>.</exception>
		public _NodesController_Base(
			NodesResolver resolver,
			ILogger<_NodesController_Base> logger)
		{
			_resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}


		/* actions */


		/// <summary>
		/// Действие по перехвату catch-all маршрута, выполняющее сопоставление входящего URL 
		/// с матрицей приоритетов файлов и рендеринг результирующего Razor-представления.
		/// </summary>
		/// <param name="path">Относительный ЧПУ-путь входящего HTTP-запроса.</param>
		/// <returns>
		/// Объект <see cref="IActionResult"/>: отрендеренное Razor-представление при успешном совпадении, 
		/// либо <see cref="NotFoundResult"/> (404), если файл не найден.
		/// </returns>
		//[HttpGet("{*path}")]
		public virtual IActionResult Render(
			string path)
		{
			var result1 = _resolver.Resolve(path);
			if (!result1.IsMatched || string.IsNullOrEmpty(result1.ViewPath))
				return NotFound();
			return View(result1.ViewPath);
		}

	}

}
