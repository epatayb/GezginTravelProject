using GezginTravel.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GezginTravel.Controllers.Admin
{
    [Authorize(Roles = RoleConstants.Admin)]
    [Route("admin/etiket")]
    public class AdminTagController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
