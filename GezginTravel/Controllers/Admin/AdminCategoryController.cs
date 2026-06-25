using GezginTravel.Constants;
using GezginTravel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GezginTravel.Controllers.Admin
{
    [Authorize(Roles = RoleConstants.Admin)]
    [Route("admin/kategori")]
    public class AdminCategoryController : Controller
    {
        private readonly GezginDbContext _context;

        public AdminCategoryController(GezginDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        public IActionResult Create(int Id)
        { 
            return View(); 
        }

        //[HttpPost("create")]
        //[ValidateAntiForgeryToken]
        //public IActionResult CreatePost()
        //{
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpGet("edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost(int id)
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
