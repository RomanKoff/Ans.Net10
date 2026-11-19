using Ans.Net10.Web;
using Ans.Net10.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Ans.Net10.CMS.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/")]
    public class NodesController(
        NodesMapper mapper)
        : _NodesController_Base(mapper)
    {

        [HttpGet("{*path}")]
        public override IActionResult Render(
            string? path)
        {
            return base.Render(path);
        }

    }

}
