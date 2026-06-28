using GezginTravel.Constants;
using GezginTravel.Data;
using GezginTravel.Models.Entities;
using GezginTravel.Models.Enums;
using GezginTravel.ViewModels.Dashboard.Admin.AdminBlog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GezginTravel.Controllers.Admin
{
    [Authorize(Roles = RoleConstants.Admin)]
    [Route("admin/blog")]
    public class AdminBlogController : Controller
    {
        private readonly GezginDbContext _context;

        public AdminBlogController(GezginDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(
            string? searchText,
            int? authorId,
            int? cityId,
            int? categoryId,
            string? status,
            string? sortBy,
            int page = 1)
        {
            const int pageSize = 10;

            if (page < 1)
            {
                page = 1;
            }

            var query = _context.Blogs
                .IgnoreQueryFilters()
                .AsQueryable();

            if (status == "Deleted")
            {
                query = query.Where(x => x.IsDeleted);
            }
            else
            {
                query = query.Where(x => !x.IsDeleted);

                if (!string.IsNullOrWhiteSpace(status) &&
                    Enum.TryParse<BlogStatus>(status, out var parsedStatus))
                {
                    query = query.Where(x => x.Status == parsedStatus);
                }
            }

            if (authorId.HasValue)
            {
                query = query.Where(x => x.AuthorId == authorId.Value);
            }

            if (cityId.HasValue)
            {
                query = query.Where(x => x.CityId == cityId.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x =>
                    x.BlogCategories.Any(bc => bc.CategoryId == categoryId.Value));
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var search = searchText.Trim();
                var likeSearch = $"%{search}%";
                var isNumericSearch = int.TryParse(search, out var searchedId);

                query = query.Where(x =>
                    (isNumericSearch && x.Id == searchedId) ||
                    EF.Functions.Like(x.Title, likeSearch) ||
                    EF.Functions.Like(x.Slug, likeSearch) ||
                    EF.Functions.Like(x.Content, likeSearch) ||
                    EF.Functions.Like(x.Author.FirstName + " " + x.Author.LastName, likeSearch) ||
                    EF.Functions.Like(x.Author.Email!, likeSearch) ||
                    (x.City != null && EF.Functions.Like(x.City.Name, likeSearch)) ||
                    (x.Country != null && EF.Functions.Like(x.Country.Name, likeSearch)) ||
                    x.BlogCategories.Any(bc => EF.Functions.Like(bc.Category.Name, likeSearch)) ||
                    x.BlogTags.Any(bt => EF.Functions.Like(bt.Tag.Name, likeSearch))
                );
            }

            query = ApplySorting(query, sortBy);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            var blogs = await query
               .Include(x => x.Author)
               .Include(x => x.City)
               .Include(x => x.Country)
               .Include(x => x.BlogCategories)
                   .ThenInclude(x => x.Category)
               .Include(x => x.BlogTags)
                   .ThenInclude(x => x.Tag)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();

            var blogItems = blogs.Select(x => new AdminBlogListItemViewModel
            {
                Id = x.Id,
                ThumbnailUrl = string.IsNullOrWhiteSpace(x.ThumbnailUrl)
                   ? ""
                   : x.ThumbnailUrl,

                Title = x.Title,
                Slug = x.Slug,

                AuthorName = $"{x.Author.FirstName} {x.Author.LastName}",

                Categories = x.BlogCategories
                   .Select(bc => bc.Category.Name)
                   .ToList(),

                Tags = x.BlogTags
                   .Select(bt => bt.Tag.Name)
                   .ToList(),

                CityName = x.City?.Name ?? "Şehir yok",
                CountryName = x.Country?.Name ?? "Ülke yok",

                ViewCount = x.ViewCount,
                LikeCount = x.LikeCount,
                CommentCount = x.CommentCount,
                SaveCount = x.SaveCount,

                TrendScore = x.TrendScore,

                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,

                Status = GetStatusText(x),
                IsDeleted = x.IsDeleted
            }).ToList();

            var model = new AdminBlogIndexViewModel
            {
                SearchText = searchText,
                SelectedAuthorId = authorId,
                SelectedCityId = cityId,
                SelectedCategoryId = categoryId,
                SelectedStatus = status,
                SelectedSortBy = sortBy,

                AuthorOptions = await GetAuthorOptionsAsync(authorId),
                CityOptions = await GetCityOptionsAsync(cityId),
                CategoryOptions = await GetCategoryOptionsAsync(categoryId),
                StatusOptions = GetStatusOptions(status),
                SortOptions = GetSortOptions(sortBy),

                Blogs = blogItems,

                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize
            };
            return View(model);
        }

        [HttpGet("detay/{id:int}")]
        public async Task<IActionResult> Detail(int id)
        {
            var blog = await _context.Blogs
                .IgnoreQueryFilters()
                .Include(x => x.Author)
                .Include(x => x.City)
                .Include(x => x.Country)
                .Include(x => x.Images)
                .Include(x => x.BlogCategories)
                    .ThenInclude(x => x.Category)
                .Include(x => x.BlogTags)
                    .ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null)
            {
                TempData["ErrorMessage"] = "Blog bulunamadı";
                return RedirectToAction(nameof(Index));
            }

            var blogViews = await _context.BlogViews
                .Include(x => x.User)
                .Where(x => x.BlogId == id)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            var recentComments = await _context.BlogComments
                .Include(x => x.User)
                .Where(x => x.BlogId == id && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .Take(5)
                .Select(x => new AdminBlogCommentItemViewModel
                {
                    UserFullName = x.User.FirstName + " " + x.User.LastName,
                    Content = x.Content,
                    CreatedDate = x.CreatedDate,
                })
                .ToListAsync();

            var actualLikeCount = await _context.BlogLikes.CountAsync(x => x.BlogId == id);
            var actualSaveCount = await _context.BlogSaves.CountAsync(x => x.BlogId == id);
            var actualCommentCount = await _context.BlogComments.CountAsync(x => x.BlogId == id && !x.IsDeleted);

            var model = new AdminBlogDetailViewModel
            {
                Id = blog.Id,
                Title = blog.Title,
                Slug = blog.Slug,
                Content = blog.Content,
                ThumbnailUrl = string.IsNullOrWhiteSpace(blog.ThumbnailUrl)
                    ? ""
                    : blog.ThumbnailUrl,

                EstimatedReadingTime = blog.EstimatedReadingTime,

                AuthorName = blog.Author.FirstName + " " + blog.Author.LastName,
                AuthorEmail = blog.Author.Email ?? "",
                AuthorTitle = blog.Author.Title,
                AuthorPhotoUrl = string.IsNullOrWhiteSpace(blog.Author.PhotoUrl)
                    ? "/uploads/users/default-user.png"
                    : blog.Author.PhotoUrl,

                CityName = blog.City?.Name ?? "Şehir bilgisi yok",
                CountryName = blog.Country?.Name ?? "Ülke bilgisi yok",

                Categories = blog.BlogCategories.Select(x => x.Category.Name).ToList(),
                Tags = blog.BlogTags.Select(x => x.Tag.Name).ToList(),
                Images = blog.Images
                    .OrderBy(x => x.OrderNo)
                    .Select(x => x.ImageUrl)
                    .ToList(),

                ViewCount = blog.ViewCount,
                BlogViewRecordCount = blogViews.Count,
                UniqueIpCount = blogViews
                    .Where(x => !string.IsNullOrWhiteSpace(x.IpAddress))
                    .Select(x => x.IpAddress)
                    .Distinct()
                    .Count(),

                RegisteredViewCount = blogViews.Count(x => x.UserId != null),
                AnonymousViewCount = blogViews.Count(x => x.UserId == null),

                LikeCount = actualLikeCount,
                CommentCount = actualCommentCount,
                SaveCount = actualSaveCount,

                InteractionScore = actualLikeCount + actualCommentCount + actualSaveCount,
                TrendScore = blog.TrendScore,

                StatusText = GetStatusText(blog),
                IsDeleted = blog.IsDeleted,

                CreatedDate = blog.CreatedDate,
                UpdatedDate = blog.UpdatedDate,
                DeletedDate = blog.DeletedDate,

                LastViewedAt = blogViews.FirstOrDefault()?.CreatedDate,
                LastCommentedAt = recentComments.FirstOrDefault()?.CreatedDate,

                RecentComments = recentComments,

                RecentViews = blogViews
                    .Take(5)
                    .Select(x => new AdminBlogViewItemViewModel
                    {
                        ViewerName = x.UserId == null
                            ? "Misafir"
                            : x.User!.FirstName + " " + x.User.LastName,
                        IpAddress = x.IpAddress ?? "",
                        CreatedDate = x.CreatedDate
                    })
                    .ToList()

            };
            return View(model);
        }

        [HttpPost("sil/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blogs
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (blog ==null)
            {
                TempData["ErrorMessage"] = "Blog bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (blog.IsDeleted)
            {
                TempData["ErrorMessage"] = "Bu blog zaten silinmiş.";
                return RedirectToAction(nameof(Detail), new { id });
            }
            blog.IsDeleted = true;
            blog.DeletedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Blog başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("geri-yukle/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var blog = await _context.Blogs
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null)
            {
                TempData["ErrorMessage"] = "Blog bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            blog.IsDeleted = false;
            blog.DeletedDate = null;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Blog tekrar aktif hale getirildi.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpGet("son-eklenen-bloglar")]
        public IActionResult LastBlogs()
        {
            return RedirectToAction(nameof(Index), new { sortBy = "created_desc" });
        }

        private static IQueryable<Blog> ApplySorting(IQueryable<Blog> query, string? sortBy)
        {
            return sortBy switch
            {
                "wiews_desc" => query.OrderByDescending(x => x.ViewCount),
                "wiews_asc" => query.OrderBy(x => x.ViewCount),

                "likes_desc" => query.OrderByDescending(x => x.LikeCount),
                "likes_asc" => query.OrderBy(x => x.LikeCount),

                "comments_desc" => query.OrderByDescending(x => x.CommentCount),
                "comments_asc" => query.OrderBy(x => x.CommentCount),

                "saves_desc" => query.OrderByDescending(x => x.SaveCount),
                "saves_asc" => query.OrderBy(x => x.SaveCount),

                "trend_desc" => query.OrderByDescending(x => x.TrendScore),
                "trend_asc" => query.OrderBy(x => x.TrendScore),

                "interaction_desc" => query.OrderByDescending(x =>
                    x.LikeCount + x.CommentCount + x.SaveCount),

                "created_desc" => query.OrderByDescending(x => x.CreatedDate),
                "created_asc" => query.OrderBy(x => x.CreatedDate),

                "updated_desc" => query.OrderByDescending(x => x.UpdatedDate ?? x.CreatedDate),
                "updated_asc" => query.OrderBy(x => x.UpdatedDate ?? x.CreatedDate),

                _ => query.OrderByDescending(x => x.CreatedDate)
            };
        }

        private async Task<List<SelectListItem>> GetAuthorOptionsAsync(int? selectedAuthorId)
        {
            var authorRoleId = await _context.Roles
                .Where(x => x.Name == RoleConstants.Author)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (authorRoleId == 0)
            {
                return new List<SelectListItem>();
            }

            var authorIds = await _context.UserRoles
                .Where(x => x.RoleId == authorRoleId)
                .Select(x => x.UserId)
                .ToListAsync();

            return await _context.Users
                .Where(x => authorIds.Contains(x.Id))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FirstName + " " + x.LastName,
                    Selected = selectedAuthorId == x.Id
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetCityOptionsAsync(int? selectedCityId)
        {
            return await _context.Cities
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = selectedCityId == x.Id
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetCategoryOptionsAsync(int? selectedCategoryId)
        {
            return await _context.Categories
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = selectedCategoryId == x.Id
                })
                .ToListAsync();
        }

        private static List<SelectListItem> GetStatusOptions(string? selectedStatus)
        {
            var options = new List<SelectListItem>
            {
                new SelectListItem { Value = "Draft", Text = "Taslak" },
                new SelectListItem { Value = "Published", Text = "Yayınlandı" },
                new SelectListItem { Value = "Archived", Text = "Arşivlendi" },
                new SelectListItem { Value = "Deleted", Text = "Silindi" },
            };

            foreach (var option in options)
            {
                option.Selected = option.Value.Equals(selectedStatus);
            }
            return options;
        }

        private static List<SelectListItem> GetSortOptions(string? selectedSortBy)
        {
            var options = new List<SelectListItem>
            {
                new SelectListItem { Value = "created_desc", Text = "En yeni oluşturulan" },
                new SelectListItem { Value = "created_asc", Text = "En eski oluşturulan" },

                new SelectListItem { Value = "updated_desc", Text = "Son güncellenen" },
                new SelectListItem { Value = "updated_asc", Text = "Eski güncellenen" },

                new SelectListItem { Value = "wiews_desc", Text = "Görüntülenme yüksekten düşüğe" },
                new SelectListItem { Value = "wiews_asc", Text = "Görüntülenme düşükten yükseğe" },

                new SelectListItem { Value = "likes_desc", Text = "Beğeni yüksekten düşüğe" },
                new SelectListItem { Value = "likes_asc", Text = "Beğeni düşükten yükseğe" },

                new SelectListItem { Value = "comments_desc", Text = "Yorum yüksekten düşüğe" },
                new SelectListItem { Value = "comments_asc", Text = "Yorum düşükten yükseğe" },

                new SelectListItem { Value = "saves_desc", Text = "Kaydetme yüksekten düşüğe" },
                new SelectListItem { Value = "saves_asc", Text = "Kaydetme düşükten yükseğe" },

                new SelectListItem { Value = "trend_desc", Text = "Trend skoru yüksekten düşüğe" },
                new SelectListItem { Value = "trend_asc", Text = "Trend skoru düşükten yükseğe" },

                new SelectListItem { Value = "interaction_desc", Text = "En çok etkileşim alan" }
            };

            foreach (var option in options)
            {
                option.Selected = option.Value == selectedSortBy;
            }

            return options;
        }

        private static string GetStatusText(Blog blog)
        {
            if (blog.IsDeleted)
            {
                return "Silindi";
            }

            return blog.Status switch
            {
                BlogStatus.Draft => "Taslak",
                BlogStatus.Published => "Yayınlandı",
                BlogStatus.Archived => "Arşivlendi",
                _ => "Bilinmiyor"
            };
        }
    }
}
