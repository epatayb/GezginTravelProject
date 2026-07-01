using GezginTravel.Constants;
using GezginTravel.Data;
using GezginTravel.Models.Entities;
using GezginTravel.ViewModels.Dashboard.Admin.AdminTag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GezginTravel.Controllers.Admin
{
    [Authorize(Roles = RoleConstants.Admin)]
    [Route("admin/tags")]
    public class AdminTagController : Controller
    {
        private readonly GezginDbContext _context;

        public AdminTagController(GezginDbContext context)
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

            var query = _context.Tags
                .Include(x => x.BlogTags)
                .AsQueryable();

            status = string.IsNullOrWhiteSpace(status)
                ? "Active"
                : status;

            query = status switch
            {
                "Deleted" => query.Where(x => x.IsDeleted),
                "all" => query,
                _ => query.Where(x => !x.IsDeleted)
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

            var tags = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AdminTagListItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    BlogCount = x.BlogTags.Count,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    IsDeleted = x.IsDeleted
                })
                .ToListAsync();

            var allTagsStats = await _context.Tags
                .Select(x=> new {x.IsDeleted, HasBlogs = x.BlogTags.Any() })
                .ToListAsync();

            var model = new AdminTagIndexViewModel
            {
                SearchText = searchText,
                Status = status,
                SortBy = sortBy,

                TotalTags = allTagsStats.Count(),
                ActiveTags = allTagsStats.Count(x => !x.IsDeleted),
                DeletedTags = allTagsStats.Count(x => x.IsDeleted),
                UsedTags = allTagsStats.Count(x => x.HasBlogs),

                Tags = tags,

                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(model);        
        }

        private static IQueryable<Tag> ApplySorting(IQueryable<Tag> query, string? sortBy)
        {
            return sortBy switch
            {
                "name_asc" => query.OrderBy(x => x.Name),
                "name_desc" => query.OrderByDescending(x => x.Name),

                "blog_desc" => query
                    .OrderByDescending(x => x.BlogTags.Count)
                    .ThenBy(x => x.Name),

                "blog_asc" => query
                    .OrderBy(x => x.BlogTags.Count)
                    .ThenBy(x => x.Name),

                "updated_desc" => query
                    .OrderByDescending(x => x.UpdatedDate ?? x.CreatedDate),

                "updated_asc" => query
                    .OrderBy(x => x.UpdatedDate ?? x.CreatedDate),

                "created_asc" => query.OrderBy(x => x.CreatedDate),

                _ => query.OrderByDescending(x => x.CreatedDate)
            };
        }

    }
}
