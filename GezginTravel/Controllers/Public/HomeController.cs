using GezginTravel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GezginTravel.Controllers.Public
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("hakkimizda")]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet("iletisim")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpGet("gizlilik")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml",
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
