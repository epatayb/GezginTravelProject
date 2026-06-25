using Microsoft.AspNetCore.Mvc;

namespace GezginTravel.Controllers.Author
{
    public class AuthorBlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
