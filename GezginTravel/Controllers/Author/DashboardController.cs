using GezginTravel.Constants;
using GezginTravel.Data;
using GezginTravel.Models.Identity;
using GezginTravel.ViewModels.Dashboard.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GezginTravel.Controllers.Author
{
    [Authorize(Roles = RoleConstants.Author)]
    [Route("author/dashboard")]
    public class DashboardController : Controller
    {
        private readonly GezginDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(GezginDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var userIdText = _userManager.GetUserId(User);

            if (!int.TryParse(userIdText, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var myBlogs = _context.Blogs
                .Where(x => x.AuthorId == userId);

            var hasBlog = await myBlogs.AnyAsync();

            var topBlog = await myBlogs
                .OrderByDescending(x => x.TrendScore)
                .Select(x => new
                {
                    x.Title,
                    x.TrendScore
                })
                .FirstOrDefaultAsync();

            var mostViewedBlog = await myBlogs
                .OrderByDescending(x => x.ViewCount)
                .Select(x => new
                {
                    x.Title,
                    x.ViewCount
                })
                .FirstOrDefaultAsync();

            var model = new AuthorDashboardViewModel
            {
                MyBlogCount = await myBlogs.CountAsync(),
                TotalViews = await myBlogs.SumAsync(x => (int?)x.ViewCount) ?? 0,
                TotalLikes = await myBlogs.SumAsync(x => (int?)x.LikeCount) ?? 0,
                TotalComments = await myBlogs.SumAsync(x => (int?)x.CommentCount) ?? 0,
                TotalSaves = await myBlogs.SumAsync(x => (int?)x.SaveCount) ?? 0,

                AverageTrendScore = hasBlog
                    ? await myBlogs.AverageAsync(x => x.TrendScore)
                    : 0,

                TopBlogTitle = topBlog?.Title,
                TopBlogTrendScore = topBlog?.TrendScore ?? 0,

                MostViewedBlogTitle = mostViewedBlog?.Title,
                MostViewedBlogCount = mostViewedBlog?.ViewCount ?? 0
            };

            return View(model);
        }
    }

}
