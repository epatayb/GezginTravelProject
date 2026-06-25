using GezginTravel.Constants;
using GezginTravel.Data;
using GezginTravel.Models.Identity;
using GezginTravel.ViewModels.Dashboard.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GezginTravel.Controllers.Admin
{
    [Authorize(Roles = RoleConstants.Admin)]
    [Route("admin/kullanici")]
    public class AdminUserController : Controller
    {
        private readonly GezginDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AdminUserController(GezginDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? searchText, string? role, int page = 1)
        {
            const int pageSize = 10;

            if (page < 1)
            {
                page = 1;
            }
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var search = searchText.Trim().ToLower();

                usersQuery = usersQuery.Where(x =>
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search) ||
                x.Email!.ToLower().Contains(search) ||
                x.UserName!.ToLower().Contains(search)
                );
            }

            var users = await usersQuery
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var roles = await _roleManager.Roles
                .OrderBy(x => x.Name)
                .Select(x => x.Name!)
                .ToListAsync();

            var blogCounts = await _context.Blogs
                .GroupBy(x => x.AuthorId)
                .Select(g => new
                {
                    AuthorId = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.AuthorId, x => x.Count);

            var allUserItems = new List<AdminUserListItemViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var primaryRole = GetPrimaryRole(userRoles);

                if (!string.IsNullOrWhiteSpace(role) && primaryRole != role)
                {
                    continue;
                }

                var isSuspended = user.LockoutEnd.HasValue &&
                                  user.LockoutEnd.Value > DateTimeOffset.UtcNow;

                allUserItems.Add(new AdminUserListItemViewModel
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    PhotoUrl = string.IsNullOrWhiteSpace(user.PhotoUrl)
                        ? "/images/default-user.png"
                        : user.PhotoUrl,
                    Roles = userRoles.ToList(),
                    PrimaryRole = primaryRole,
                    TotalBlogs = blogCounts.ContainsKey(user.Id)
                        ? blogCounts[user.Id]
                        : 0,
                    CreatedAt = user.CreatedAt,
                    PopularityScore = user.PopularityScore,
                    IsSuspended = isSuspended
                });
            }

            var totalItems = allUserItems.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            var pagedUsers = allUserItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new AdminUserIndexViewModel
            {
                SearchText = searchText,
                SelectedRole = role,

                TotalUsers = allUserItems.Count,
                ActiveUsers = allUserItems.Count(x => !x.IsSuspended),
                SuspendedUsers = allUserItems.Count(x => x.IsSuspended),
                TotalAuthors = allUserItems.Count(x => x.Roles.Contains(RoleConstants.Author)),

                Roles = roles,
                Users = pagedUsers,

                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
            return View(model);
        }

        [HttpGet("ekle")]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpGet("son-eklenen-kullanicilar")]
        public IActionResult LastUsers()
        {
            return View();
        }

        [HttpGet("detay/{id:int}")]
        public async Task<IActionResult> Detail(int id)
        {
            var user= await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpGet("duzenle/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpPost("disable/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == user.Id.ToString())
            {
                TempData["ErrorMessage"] = "Kendi hesabınızı pasifleştiremezsiniz.";
                return RedirectToAction(nameof(Index));
            }

            user.LockoutEnd = DateTimeOffset.MaxValue;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Kullanıcı pasifleştirildi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı pasifleştirilirken hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("restore/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            user.LockoutEnd = null;
            user.AccessFailedCount = 0;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Kullanıcı tekrar aktif edildi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı aktif edilirken hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        private static string GetPrimaryRole(IEnumerable<string> roles)
        {
            var rolesList = roles.ToList();

            if (rolesList.Contains(RoleConstants.Admin))
            {
                return RoleConstants.Admin;
            }
            if (rolesList.Contains(RoleConstants.Author))
            {
                return RoleConstants.Author;
            }
            if (rolesList.Contains(RoleConstants.User))
            {
                return RoleConstants.User;
            }

            return "Rol Bulunuamadı";
        }
    }
}
