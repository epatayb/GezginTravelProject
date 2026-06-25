using Microsoft.AspNetCore.Mvc;

namespace GezginTravel.Controllers.Public
{
    [Route("rotalar")]
    public class RoutesController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
