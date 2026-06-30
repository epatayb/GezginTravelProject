using GezginTravel.Constants;
using GezginTravel.Data;
using GezginTravel.Models.Entities;
using GezginTravel.ViewModels.Dashboard.Admin.AdminCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index(
            string? searchText,
            string status = "Active",
            string? sortBy = "created_desc",
            int page = 1)

        {
            const int pageSize = 10;

            if (page < 1)
            {
                page = 1;
            }

            var query = _context.Categories
                .Include(x => x.BlogCategories)
                .AsQueryable();

            status = string.IsNullOrWhiteSpace(status)
                ? "Active"
                : status;

            query = status switch
            {
                "Deleted" => query.Where(x => x.IsDeleted),
                "all" => query,
                _ => query.Where(x => !x.IsDeleted),
            };

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var search = searchText.Trim();
                var likeSearch = $"%{search}%";

                query = query.Where(x =>
                    EF.Functions.Like(x.Name, likeSearch) ||
                    EF.Functions.Like(x.Slug, likeSearch));
            }

            query = ApplySorting(query, sortBy);

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            var categories = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AdminCategoryListItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    BlogCount = x.BlogCategories.Count,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    IsDeleted = x.IsDeleted
                })
                .ToListAsync();

            var model = new AdminCategoryIndexViewModel
            {
                SearchText = searchText,
                Status = status,
                SortBy = sortBy,

                TotalCategories = await _context.Categories.CountAsync(),
                ActiveCategories = await _context.Categories.CountAsync(x => !x.IsDeleted),
                DeletedCategories = await _context.Categories.CountAsync(x => x.IsDeleted),
                UsedCategories = await _context.Categories.CountAsync(x => x.BlogCategories.Any()),

                Categories = categories,

                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
            };
            return View(model);
        }

        [HttpGet("ekle")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("ekle")]
        public IActionResult Create(int Id)
        {
            return View();
        }

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

        private static IQueryable<Category> ApplySorting(IQueryable<Category> query, string? sortBy)
        {
            return sortBy switch
            {
                "name_asc" => query.OrderBy(x => x.Name),
                "name_desc" => query.OrderByDescending(x => x.Name),

                "blog_asc" => query.OrderBy(x => x.BlogCategories.Count).ThenBy(x => x.Name),
                "blog_desc" => query.OrderByDescending(x => x.BlogCategories.Count).ThenBy(x => x.Name),

                "updated_asc" => query.OrderBy(x => x.UpdatedDate ?? x.CreatedDate),
                "updated_desc" => query.OrderByDescending(x => x.UpdatedDate ?? x.CreatedDate),

                "created_asc" => query.OrderBy(x => x.CreatedDate),

                _ => query.OrderByDescending(x => x.CreatedDate),
            };
        }

    }
}
