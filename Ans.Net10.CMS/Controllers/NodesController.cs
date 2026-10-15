using Microsoft.AspNetCore.Mvc;

namespace Ans.Net10.CMS.Controllers
{

	[Route("/")]
	public class NodesController
		: Controller
	{

		[HttpGet("{*path}")]
		public IActionResult Index(
			string path)
		{
			return View();
		}

	}

}
