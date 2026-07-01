using GezginTravel.Constants;
using GezginTravel.Data;
using GezginTravel.Helpers;
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
            return View(new AdminCategoryCreateViewModel());
        }

        [HttpPost("ekle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var slug = SlugHelper.GenerateSlug(model.Name);

            var slugExists = await _context.Categories
                .AnyAsync(x => x.Slug == slug);

            if (slugExists)
            {
                ModelState.AddModelError(nameof(model.Name), "Bu kategori adı daha önce kullanılmış. Silinmiş kategorilerde de aynı slug bulunabilir.");
                return View(model);
            }

            var category = new Category
            {
                Name = model.Name.Trim(),
                Slug = slug
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kategori başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));

        }

        [HttpGet("duzenle/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories
                .FindAsync(id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var model = new AdminCategoryEditViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug
            };

            return View(model);
        }

        [HttpPost("duzenle/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminCategoryEditViewModel model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz kategori isteği";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = await _context.Categories
                .FindAsync(id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var trimmedName = model.Name.Trim();
            if (category.Name != trimmedName)
            {
                var newSlug = SlugHelper.GenerateSlug(trimmedName);

                var slugExists = await _context.Categories
                    .AnyAsync(x => x.Id != id && x.Slug == newSlug);

                if (slugExists)
                {
                    ModelState.AddModelError(nameof(model.Name), "Bu kategori adı başka bir kategoride kullanılıyor.");
                    return View(model);
                }

                category.Name = trimmedName;
                category.Slug = newSlug;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "kategori başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("sil/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (category.IsDeleted)
            {
                TempData["ErrorMessage"] = "Bu kategori zaten silinmiş durumda.";
                return RedirectToAction(nameof(Index));
            }

            category.IsDeleted = true;
            category.DeletedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("geri-yukle/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            category.IsDeleted = false;
            category.DeletedDate = null;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kategori tekrar aktif hale getirildi.";
            return RedirectToAction(nameof(Index), new { status = "Active" });
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
