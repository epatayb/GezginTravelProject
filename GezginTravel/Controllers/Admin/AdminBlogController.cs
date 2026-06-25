using GezginTravel.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GezginTravel.Controllers.Admin
{
    [Authorize(Roles = RoleConstants.Admin)]
    [Route("admin/blog")]
    public class AdminBlogController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("son-eklenen-bloglar")]
        public IActionResult LastBlogs ()
        {
            return View();
        }
    }
}
