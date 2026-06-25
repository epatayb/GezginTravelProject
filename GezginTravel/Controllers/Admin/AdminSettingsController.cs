using GezginTravel.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GezginTravel.Controllers.Admin
{
    [Authorize(Roles = RoleConstants.Admin)]
    [Route("admin/ayarlar")]
    public class AdminSettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
