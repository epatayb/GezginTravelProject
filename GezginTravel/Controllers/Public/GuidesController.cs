using Microsoft.AspNetCore.Mvc;

namespace GezginTravel.Controllers.Public
{
    [Route("rehberler")]
    public class GuidesController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
