using Ans.Net10.Web.Controllers;
using Ans.Net10.Web.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace Ans.Net10.CMS.Controllers
{

	/// <summary>
	/// Централизованный CMS-контроллер, обеспечивающий перехват динамических ЧПУ-маршрутов, 
	/// их разрешение через внутренний реестр представлений и рендеринг контента.
	/// </summary>	
	[ApiExplorerSettings(IgnoreApi = true)]
	[Route("/")]
	public class NodesController
		: _NodesController_Base
	{

		/* ctor */


		/// <inheritdoc/>
		public NodesController(
			NodesResolver resolver,
			ILogger<NodesController> logger)
			: base(resolver, logger)
		{
		}


		/* actions */


		/// <inheritdoc/>
		[HttpGet("{*path}")]
		public override IActionResult Render(
			string path)
		{
			return base.Render(path);
		}

	}

}
