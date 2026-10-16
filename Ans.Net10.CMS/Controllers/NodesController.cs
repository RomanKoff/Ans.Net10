using Ans.Net10.Web.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace Ans.Net10.CMS.Controllers
{

	/// <summary>
	/// Централизованный CMS-контроллер, обеспечивающий перехват динамических ЧПУ-маршрутов, 
	/// их разрешение через внутренний реестр представлений и рендеринг контента.
	/// </summary>
	[Route("/")]
	public class NodesController
		: Controller
	{

		private readonly NodesResolver _resolver;
		private readonly ILogger<NodesController> _logger;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NodesController"/>.
		/// </summary>
		/// <param name="resolver">Служба анализа и разрешения путей CMS в памяти.</param>
		/// <param name="logger">Служба ведения системных логов.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если один из входящих параметров равен <see langword="null"/>.</exception>
		public NodesController(
			NodesResolver resolver,
			ILogger<NodesController> logger)
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
		[HttpGet("{*path}")]
		public IActionResult Render(
			string path)
		{
			var result1 = _resolver.Resolve(path);
			if (!result1.IsMatched || string.IsNullOrEmpty(result1.ViewPath))
				return NotFound();
			return View(result1.ViewPath);
		}

	}

}
