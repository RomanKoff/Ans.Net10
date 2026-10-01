using Microsoft.AspNetCore.Mvc;

namespace _test_web.Controllers
{

	[Route("/")]
	public class DefaultController
		: Controller
	{

		[Route("")]
		public IActionResult Index()
		{
			return View();
		}

	}

}
