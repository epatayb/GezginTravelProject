using GezginTravel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GezginTravel.Controllers.Public
{
    [Route("kesfet")]
    public class ExploreController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
