using Ans.Net10.Web.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace _test_web.Controllers
{

	/// <summary>
	/// Универсальный инфраструктурный контроллер-рендерер, обеспечивающий 
	/// асинхронную обработку, сопоставление и отображение динамических Razor-представлений CMS.
	/// </summary>
	[ApiExplorerSettings(IgnoreApi = true)]
	public sealed partial class NodesController
		: Controller
	{

		private readonly INodePathResolver _resolver;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NodesController"/> 
		/// с внедрением сервиса анализа путей.
		/// </summary>
		/// <param name="resolver">Сервис анализа и сопоставления путей в оперативной памяти.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="resolver"/> равен <see langword="null"/>.
		/// </exception>
		public NodesController(
			INodePathResolver resolver)
		{
			_resolver = resolver
				?? throw new ArgumentNullException(nameof(resolver));
		}


		/* actions */


		/// <summary>
		/// Асинхронно анализирует входящий относительный путь запроса, производит сопоставление 
		/// с картой представлений в памяти и выполняет рендеринг целевой Razor-страницы Узла.
		/// </summary>
		/// <param name="path">Относительный ЧПУ-путь к запрашиваемой странице из Catch-all маршрута.</param>
		/// <returns>
		/// Объект <see cref="IActionResult"/>, представляющий результат рендеринга страницы, 
		/// либо <see cref="NotFoundResult"/>, если совпадений в карте физических представлений не найдено.
		/// </returns>
		[HttpGet("{*path}")]
		public async Task<IActionResult> RenderPage(
			string? path)
		{
			var resolveResult1 = await _resolver.ResolveAsync(path);
			if (resolveResult1 == null)
				return NotFound();
			HttpContext.Features.Set<INodePageFeature>(
				new NodePageFeature(resolveResult1));
			ViewData["CurrentNodeName"] = resolveResult1.NodeName;
			ViewData["CurrentPagePath"] = resolveResult1.PagePath;
			ViewData["IsStartPage"] = resolveResult1.IsStartPage;
			return View(resolveResult1.ViewPath);
		}

	}

}
