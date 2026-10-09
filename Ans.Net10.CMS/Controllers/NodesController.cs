using Microsoft.AspNetCore.Mvc;

namespace Ans.Net10.CMS.Controllers
{

	[ApiExplorerSettings(IgnoreApi = true)]
	[Route("/")]
	public sealed class NodesController
		: Controller
	{

		[HttpGet("")]
		public async Task<IActionResult> Index()
		{
			return View();
		}


		[HttpGet("{*path}")]
		public async Task<IActionResult> RenderPage(
			string? path)
		{
			return NotFound();
		}


		[HttpGet("bad")]
		public async Task<IActionResult> Bad()
		{
			var i1 = 0;
			var i2 = 0;
			var i3 = i1 / i2;
			return View(i3);
		}

	}

}
