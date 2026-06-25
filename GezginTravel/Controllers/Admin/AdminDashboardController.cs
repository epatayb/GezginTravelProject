using GezginTravel.Constants;
using GezginTravel.Data;
using GezginTravel.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using GezginTravel.ViewModels.Dashboard.Admin;

namespace GezginTravel.Controllers.Admin
{
    [Authorize(Roles = RoleConstants.Admin)]
    [Route("admin/dashboard")]
    public class AdminDashboardController : Controller
    {
        private readonly GezginDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AdminDashboardController(GezginDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var authorId = await _context.Roles
                .Where(x => x.FullName == RoleConstants.Author)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var mostContentCity = await _context.Blogs
                .Where(x => x.CityId != null)
                .GroupBy(x => new
                {
                    x.CityId,
                    x.City.Name,
                    CountryName = x.City.Country.Name
                })
                .Select(g => new
                {
                    CityName = g.Key.Name,
                    g.Key.CountryName,
                    BlogCount = g.Count()
                })
                .OrderByDescending(x => x.BlogCount)
                .FirstOrDefaultAsync();

            var mostContentCountry = await _context.Blogs
                .Where(x => x.CountryId != null)
                .GroupBy(x => new
                {
                    x.CountryId,
                    x.Country.Name
                })
                .Select(g => new
                {
                    CountryName = g.Key.Name,
                    BlogCount = g.Count()
                })
                .OrderByDescending(x => x.BlogCount)
                .FirstOrDefaultAsync();

            var topTrendBlog = await _context.Blogs
                .OrderByDescending(x => x.TrendScore)
                .Select(x => new
                {
                    x.Title,
                    x.TrendScore
                })
                .FirstOrDefaultAsync();

            var mostViewedBlog = await _context.Blogs
                .OrderByDescending(x => x.ViewCount)
                .Select(x => new
                {
                    x.Title,
                    x.ViewCount
                })
                .FirstOrDefaultAsync();

            var mostLikedBlog = await _context.Blogs
                .OrderByDescending(x => x.LikeCount)
                .Select(x => new
                {
                    x.Title,
                    x.LikeCount
                })
                .FirstOrDefaultAsync();

            var mostSavedBlog = await _context.Blogs
                .OrderByDescending(x => x.SaveCount)
                .Select(x => new
                {
                    x.Title,
                    x.SaveCount
                })
                .FirstOrDefaultAsync();

            var mostCommentBlog = await _context.Blogs
                .OrderByDescending(x => x.CommentCount)
                .Select(x => new
                {
                    x.Title,
                    x.CommentCount
                })
                .FirstOrDefaultAsync();

            var topAuthor = await _context.Blogs
                .GroupBy(x => new
                {
                    x.AuthorId,
                    x.Author.FirstName,
                    x.Author.LastName,
                    x.Author.PhotoUrl
                })
                .Select(g => new
                {
                    FullName = g.Key.FirstName + " " + g.Key.LastName,
                    g.Key.PhotoUrl,
                    TotalViewed = g.Sum(x => x.ViewCount),
                    AverageTrendScore = g.Average(x => x.TrendScore)
                })
                .OrderByDescending(x => x.AverageTrendScore)
                .FirstOrDefaultAsync();

            var mostUsedCategory = await _context.BlogCategories
                .GroupBy(x => new
                {
                    x.CategoryId,
                    x.Category.Name
                })
                .Select(g => new
                {
                    CategoryName = g.Key.Name,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            var mostUsedTag = await _context.BlogTags
                .GroupBy(x => new
                {
                    x.TagId,
                    x.Tag.Name
                })
                .Select(g => new
                {
                    TagName = g.Key.Name,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            var recentBlogs = await _context.Blogs
                .Include(x => x.Author)
                .Include(x => x.City)
                .OrderByDescending(x => x.CreatedDate)
                .Take(5)
                .Select(x => new RecentBlogViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    AuthorName = x.Author.FirstName + " " + x.Author.LastName,
                    CityName = x.City != null ? x.City.Name : "Şehir bilgisi girilmemiş",
                    TrendScore = x.TrendScore,
                    CreatedDate = x.CreatedDate
                })
                .ToListAsync();

            var lastUsers = await _context.Users
                .OrderByDescending(x => x.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentUsers = new List<RecentUserViewModel>();

            foreach (var user in lastUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = GetPrimaryRole(roles);

                recentUsers.Add(new RecentUserViewModel
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email ?? "",
                    RoleText = primaryRole,
                    PhotoUrl = string.IsNullOrWhiteSpace(user.PhotoUrl)
                        ? "/uploads/users/default-user.png"
                        : user.PhotoUrl,
                    CreatedAt = user.CreatedAt
                });
            }

            var model = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalAuthors = await _context.UserRoles.CountAsync(x => x.RoleId == authorId),
                TotalBlogs = await _context.Blogs.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                TotalCities = await _context.Cities.CountAsync(),
                TotalCountries = await _context.Countries.CountAsync(),
                TotalComments = await _context.BlogComments.CountAsync(),

                TotalViews = await _context.Blogs.SumAsync(x => (int?)x.ViewCount) ?? 0,
                TotalLikes = await _context.Blogs.SumAsync(x => (int?)x.LikeCount) ?? 0,
                TotalSaves = await _context.Blogs.SumAsync(x => (int?)x.SaveCount) ?? 0,

                MostContentCityName = mostContentCity?.CityName,
                MostContentCityCountryName = mostContentCity?.CountryName,
                MostContentCityBlogCount = mostContentCity?.BlogCount ?? 0,

                MostContentCountryName = mostContentCountry?.CountryName,
                MostContentCountryBlogCount = mostContentCountry?.BlogCount ?? 0,

                TopTrendBlogTitle = topTrendBlog?.Title,
                TopTrendBlogScore = topTrendBlog?.TrendScore ?? 0,

                MostViewedBlogTitle = mostViewedBlog?.Title,
                MostViewedBlogCount = mostViewedBlog?.ViewCount ?? 0,

                MostSavedBlogTitle = mostSavedBlog?.Title,
                MostSavedBlogCount = mostSavedBlog?.SaveCount ?? 0,

                MostLikedBlogTitle = mostLikedBlog?.Title,
                MostLikedBlogCount = mostLikedBlog?.LikeCount ?? 0,

                MostCommentBlogTitle = mostCommentBlog?.Title,
                MostCommentBlogCount = mostCommentBlog?.CommentCount ?? 0,

                TopAuthorName = topAuthor?.FullName,
                TopAuthorPhotoUrl = topAuthor?.PhotoUrl,
                TopAuthorTotalViewed = topAuthor != null ? topAuthor.TotalViewed : 0,
                TopAuthorDisplayViewed = topAuthor != null ? FormatViewCount(topAuthor.TotalViewed) : "0",
                TopAuthorScore = topAuthor?.AverageTrendScore ?? 0,

                MostUsedCategoryName = mostUsedCategory?.CategoryName,
                MostUsedCategoryCount = mostUsedCategory?.Count ?? 0,

                MostUsedTagName = mostUsedTag?.TagName,
                MostUsedTagCount = mostUsedTag?.Count ?? 0,

                RecentBlogs = recentBlogs,
                RecentUsers = recentUsers
            };

            return View(model);
        }

        public string FormatViewCount(decimal viewCount)
        {
            if (viewCount >= 1000)
            {
                return (viewCount / 1000m).ToString("0.#") + "k";
            }
            return viewCount.ToString("0");
        }

        private static string GetPrimaryRole(IEnumerable<string> roles)
        {
            var roleList = roles.ToList();

            if (roleList.Contains(RoleConstants.Admin))
            {
                return RoleConstants.Admin;
            }

            if (roleList.Contains(RoleConstants.Author))
            {
                return RoleConstants.Author;
            }

            if (roleList.Contains(RoleConstants.User))
            {
                return RoleConstants.User;
            }

            return "Rol yok";
        }
    }
}
