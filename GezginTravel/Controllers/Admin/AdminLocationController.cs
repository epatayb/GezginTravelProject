using GezginTravel.Constants;
using GezginTravel.Data;
using GezginTravel.Helpers;
using GezginTravel.Models.Entities;
using GezginTravel.ViewModels.Dashboard.Admin.AdminLocation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GezginTravel.Controllers.Admin
{
    [Authorize(Roles = RoleConstants.Admin)]
    [Route("admin/lokasyon")]
    public class AdminLocationController : Controller
    {
        private readonly GezginDbContext _context;

        public AdminLocationController(GezginDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(
            string? searchText,
            string status = "active",
            string? sortBy = "created_desc",
            int? countryId = null)
        {
            status = string.IsNullOrWhiteSpace(status)
                ? "active"
                : status;

            var countryQuery = _context.Countries
                .Include(x => x.Cities)
                .Include(x => x.Blogs)
                .AsQueryable();

            var cityQuery = _context.Cities
                .Include(x => x.Country)
                .Include(x => x.Blogs)
                .AsQueryable();

            countryQuery = ApplyStatusFilter(countryQuery, status);
            cityQuery = ApplyStatusFilter(cityQuery, status);

            if (countryId.HasValue)
            {
                cityQuery = cityQuery.Where(x => x.CountryId == countryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var search = searchText.Trim();
                var likeSearch = $"%{search}%";

                countryQuery = countryQuery.Where(x =>
                    EF.Functions.Like(x.Name, likeSearch) ||
                    EF.Functions.Like(x.Slug, likeSearch));

                cityQuery = cityQuery.Where(x =>
                    EF.Functions.Like(x.Name, likeSearch) ||
                    EF.Functions.Like(x.Slug, likeSearch) ||
                    EF.Functions.Like(x.Country.Name, likeSearch) ||
                    EF.Functions.Like(x.Country.Slug, likeSearch));
            }

            countryQuery = ApplyCountrySorting(countryQuery, sortBy);
            cityQuery = ApplyCitySorting(cityQuery, sortBy);

            var countries = await countryQuery
                .Select(x => new AdminCountryListItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    CityCount = x.Cities.Count(x => !x.IsDeleted),
                    BlogCount = x.Blogs.Count(x => !x.IsDeleted),
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    IsDeleted = x.IsDeleted,
                })
                .ToListAsync();

            var cities = await cityQuery
                .Select(x => new AdminCityListItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    CountryId = x.CountryId,
                    CountryName = x.Country.Name,
                    BlogCount = x.Blogs.Count(x => !x.IsDeleted),
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    IsDeleted = x.IsDeleted,
                    IsCountryDeleted = x.Country.IsDeleted
                })
                .ToListAsync();

            var topBlogCity = await _context.Blogs
                .IgnoreQueryFilters()
                .Where(x => !x.IsDeleted && !x.City.IsDeleted)
                .GroupBy(x => new
                {
                    x.CityId,
                    CityName = x.City.Name,
                })
                .Select(x => new
                {
                    Name = x.Key.CityName,
                    Count = x.Count()
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            var topBlogCountry = await _context.Blogs
                .IgnoreQueryFilters()
                .Where(x => !x.IsDeleted && !x.Country.IsDeleted)
                .GroupBy(x => new
                {
                    x.CountryId,
                    CountryName = x.Country.Name
                })
                .Select(x => new
                {
                    Name = x.Key.CountryName,
                    Count = x.Count()
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            var mostCitiesCountry = await _context.Countries
                .Where(x => !x.IsDeleted)
                .Select(x => new
                {
                    x.Name,
                    CityCount = x.Cities.Count(c => !c.IsDeleted)
                })
                .OrderByDescending(x => x.CityCount)
                .ThenBy(x => x.Name)
                .FirstOrDefaultAsync();

            var model = new AdminLocationIndexViewModel
            {
                SearchText = searchText,
                Status = status,
                SortBy = sortBy,
                SelectedCountryId = countryId,

                TotalCountries = await _context.Countries.CountAsync(),
                ActiveCountries = await _context.Countries.CountAsync(x => !x.IsDeleted),
                DeletedCountries = await _context.Countries.CountAsync(x => x.IsDeleted),

                MostCitiesCountryName = mostCitiesCountry?.Name ?? "-",
                MostCitiesCountryCount = mostCitiesCountry?.CityCount ?? 0,

                TotalCities = await _context.Cities.CountAsync(),
                ActiveCities = await _context.Cities.CountAsync(x => !x.IsDeleted),
                DeletedCities = await _context.Cities.CountAsync(x => x.IsDeleted),

                UsedCities = await _context.Cities
                    .CountAsync(x => x.Blogs.Any(x => !x.IsDeleted)),

                TopBlogCityName = topBlogCity?.Name ?? "-",
                TopBlogCityCount = topBlogCity?.Count ?? 0,

                TopBlogCountryName = topBlogCountry?.Name ?? "-",
                TopBlogCountryCount = topBlogCountry?.Count ?? 0,

                CountryOptions = await GetCountryOptionsAsync(countryId),

                Countries = countries,
                Cities = cities
            };

            return View(model);
        }

        [HttpGet("ulke/ekle")]
        public IActionResult CreateCountry()
        {
            return View(new AdminCountryCreateViewModel());
        }

        [HttpPost("ulke/ekle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCountry(AdminCountryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var slug = SlugHelper.GenerateSlug(model.Name);

            var slugExists = await _context.Countries
                .AnyAsync(x => x.Slug == slug);

            if (slugExists)
            {
                ModelState.AddModelError(nameof(model.Name), "Bu ülke adı daha önce kullanılmış.");
                return View(model);
            }

            var country = new Country
            {
                Name = model.Name.Trim(),
                Slug = slug
            };

            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ülke başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("ulke/duzenle/{id:int}")]
        public async Task<IActionResult> EditCountry(int id)
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(x => x.Id == id);

            if (country == null)
            {
                TempData["ErrorMessage"] = "Ülke bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var model = new AdminCountryEditViewModel
            {
                Id = country.Id,
                Name = country.Name,
                Slug = country.Slug
            };

            return View(model);
        }

        [HttpPost("ulke/duzenle/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCountry(int id, AdminCountryEditViewModel model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz ülke isteği.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(x => x.Id == id);

            if (country == null)
            {
                TempData["ErrorMessage"] = "Ülke bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var newSlug = SlugHelper.GenerateSlug(model.Name);

            var slugExists = await _context.Countries
                .AnyAsync(x => x.Id != id && x.Slug == newSlug);

            if (slugExists)
            {
                ModelState.AddModelError(nameof(model.Name), "Bu ülke adı başka bir kayıtta kullanılıyor.");
                return View(model);
            }

            country.Name = model.Name.Trim();
            country.Slug = newSlug;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ülke başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("ulke/sil/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(x => x.Id == id);

            if (country == null)
            {
                TempData["ErrorMessage"] = "Ülke bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (country.IsDeleted)
            {
                TempData["ErrorMessage"] = "Bu ülke zaten silinmiş.";
                return RedirectToAction(nameof(Index));
            }

            var activeCityCount = await _context.Cities
                .CountAsync(x => x.CountryId == id && !x.IsDeleted);

            var activeBlogCount = await _context.Blogs
                .IgnoreQueryFilters()
                .CountAsync(x => x.CountryId == id && !x.IsDeleted);

            if (activeCityCount > 0)
            {
                TempData["ErrorMessage"] = "Bu ülkeye bağlı aktif şehir bulunduğu için ülke silinemez.";
                return RedirectToAction(nameof(Index));
            }

            if (activeBlogCount > 0)
            {
                TempData["ErrorMessage"] = "Bu ülkeye bağlı aktif blog bulunduğu için ülke silinemez.";
                return RedirectToAction(nameof(Index));
            }

            country.IsDeleted = true;
            country.DeletedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ülke başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("ulke/geri-yukle/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreCountry(int id)
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(x => x.Id == id);

            if (country == null)
            {
                TempData["ErrorMessage"] = "Ülke bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            country.IsDeleted = false;
            country.DeletedDate = null;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ülke tekrar aktif hale getirildi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("sehir/ekle")]
        public async Task<IActionResult> CreateCity()
        {
            var model = new AdminCityCreateViewModel
            {
                CountryOptions = await GetCountryOptionsAsync()
            };

            return View(model);
        }

        [HttpPost("sehir/ekle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCity(AdminCityCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CountryOptions = await GetCountryOptionsAsync(model.CountryId);
                return View(model);
            }

            var countryExists = await _context.Countries
                .AnyAsync(x => x.Id == model.CountryId && !x.IsDeleted);

            if (!countryExists)
            {
                ModelState.AddModelError(nameof(model.CountryId), "Geçerli bir aktif ülke seçiniz.");
                model.CountryOptions = await GetCountryOptionsAsync(model.CountryId);
                return View(model);
            }

            var slug = SlugHelper.GenerateSlug(model.Name);

            var slugExists = await _context.Cities
                .AnyAsync(x =>
                    x.CountryId == model.CountryId &&
                    x.Slug == slug);

            if (slugExists)
            {
                ModelState.AddModelError(nameof(model.Name), "Bu şehir adı seçilen ülke içinde daha önce kullanılmış.");
                model.CountryOptions = await GetCountryOptionsAsync(model.CountryId);
                return View(model);
            }

            var city = new City
            {
                Name = model.Name.Trim(),
                Slug = slug,
                CountryId = model.CountryId!.Value
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Şehir başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index), new { countryId = city.CountryId });
        }

        [HttpGet("sehir/duzenle/{id:int}")]
        public async Task<IActionResult> EditCity(int id)
        {
            var city = await _context.Cities
                .FirstOrDefaultAsync(x => x.Id == id);

            if (city == null)
            {
                TempData["ErrorMessage"] = "Şehir bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var model = new AdminCityEditViewModel
            {
                Id = city.Id,
                Name = city.Name,
                Slug = city.Slug,
                CountryId = city.CountryId,
                CountryOptions = await GetCountryOptionsAsync(city.CountryId)
            };

            return View(model);
        }

        [HttpPost("sehir/duzenle/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCity(int id, AdminCityEditViewModel model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz şehir isteği.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.CountryOptions = await GetCountryOptionsAsync(model.CountryId);
                return View(model);
            }

            var city = await _context.Cities
                .FirstOrDefaultAsync(x => x.Id == id);

            if (city == null)
            {
                TempData["ErrorMessage"] = "Şehir bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var countryExists = await _context.Countries
                .AnyAsync(x => x.Id == model.CountryId && !x.IsDeleted);

            if (!countryExists)
            {
                ModelState.AddModelError(nameof(model.CountryId), "Geçerli bir aktif ülke seçiniz.");
                model.CountryOptions = await GetCountryOptionsAsync(model.CountryId);
                return View(model);
            }

            var newSlug = SlugHelper.GenerateSlug(model.Name);

            var slugExists = await _context.Cities
                .AnyAsync(x =>
                    x.Id != id &&
                    x.CountryId == model.CountryId &&
                    x.Slug == newSlug);

            if (slugExists)
            {
                ModelState.AddModelError(nameof(model.Name), "Bu şehir adı seçilen ülke içinde başka bir kayıtta kullanılıyor.");
                model.CountryOptions = await GetCountryOptionsAsync(model.CountryId);
                return View(model);
            }

            city.Name = model.Name.Trim();
            city.Slug = newSlug;
            city.CountryId = model.CountryId!.Value;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Şehir başarıyla güncellendi.";
            return RedirectToAction(nameof(Index), new { countryId = city.CountryId });
        }

        [HttpPost("sehir/sil/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities
                .FirstOrDefaultAsync(x => x.Id == id);

            if (city == null)
            {
                TempData["ErrorMessage"] = "Şehir bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (city.IsDeleted)
            {
                TempData["ErrorMessage"] = "Bu şehir zaten silinmiş.";
                return RedirectToAction(nameof(Index));
            }

            var activeBlogCount = await _context.Blogs
                .IgnoreQueryFilters()
                .CountAsync(x => x.CityId == id && !x.IsDeleted);

            if (activeBlogCount > 0)
            {
                TempData["ErrorMessage"] = "Bu şehre bağlı aktif blog olduğu için şehir silinemez.";
                return RedirectToAction(nameof(Index), new { countryId = city.CountryId });
            }

            city.IsDeleted = true;
            city.DeletedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Şehir başarıyla silindi.";
            return RedirectToAction(nameof(Index), new { countryId = city.CountryId });
        }

        [HttpPost("sehir/geri-yukle/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreCity(int id)
        {
            var city = await _context.Cities
                .Include(x => x.Country)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (city == null)
            {
                TempData["ErrorMessage"] = "Şehir bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (city.Country.IsDeleted)
            {
                TempData["ErrorMessage"] = "Bu şehrin ülkesi silinmiş durumda. Önce ülkeyi geri yükleyiniz.";
                return RedirectToAction(nameof(Index), new { status = "deleted" });
            }

            city.IsDeleted = false;
            city.DeletedDate = null;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Şehir tekrar aktif hale getirildi.";
            return RedirectToAction(nameof(Index), new { countryId = city.CountryId });
        }

        private static IQueryable<Country> ApplyStatusFilter(
            IQueryable<Country> query,
            string status)
        {
            return status switch
            {
                "deleted" => query.Where(x => x.IsDeleted),
                "all" => query,
                _ => query.Where(x => !x.IsDeleted)
            };
        }

        private static IQueryable<City> ApplyStatusFilter(
            IQueryable<City> query,
            string status)
        {
            return status switch
            {
                "deleted" => query.Where(x => x.IsDeleted),
                "all" => query,
                _ => query.Where(x => !x.IsDeleted)
            };
        }

        private static IQueryable<Country> ApplyCountrySorting(
            IQueryable<Country> query,
            string? sortBy)
        {
            return sortBy switch
            {
                "name_asc" => query.OrderBy(x => x.Name),
                "name_desc" => query.OrderByDescending(x => x.Name),

                "city_desc" => query
                    .OrderByDescending(x => x.Cities.Count(x => !x.IsDeleted))
                    .ThenBy(x => x.Name),

                "city_asc" => query
                    .OrderBy(x => x.Cities.Count(y => !y.IsDeleted))
                    .ThenBy(x => x.Name),

                "blog_desc" => query
                    .OrderByDescending(x => x.Blogs.Count(y => !y.IsDeleted))
                    .ThenBy(x => x.Name),

                "blog_asc" => query
                    .OrderBy(x => x.Blogs.Count(x => !x.IsDeleted))
                    .ThenBy(x => x.Name),

                "updated_desc" => query
                    .OrderByDescending(x => x.UpdatedDate ?? x.CreatedDate),

                "created_asc" => query.OrderBy(x => x.CreatedDate),
                "created_desc" => query.OrderByDescending(x => x.CreatedDate),

                _ => query
                    .OrderByDescending(x => x.Blogs.Count(y => !y.IsDeleted))
                    .ThenBy(x => x.Name)
            };
        }

        private static IQueryable<City> ApplyCitySorting(
            IQueryable<City> query,
            string? sortBy)
        {
            return sortBy switch
            {
                "name_asc" => query.OrderBy(x => x.Name),
                "name_desc" => query.OrderByDescending(x => x.Name),

                "blog_desc" => query
                    .OrderByDescending(x => x.Blogs.Count(b => !b.IsDeleted))
                    .ThenBy(x => x.Name),

                "blog_asc" => query
                    .OrderBy(x => x.Blogs.Count(b => !b.IsDeleted))
                    .ThenBy(x => x.Name),

                "updated_desc" => query
                    .OrderByDescending(x => x.UpdatedDate ?? x.CreatedDate),

                "created_asc" => query.OrderBy(x => x.CreatedDate),
                "created_desc" => query.OrderByDescending(x => x.CreatedDate),

                _ => query
                    .OrderByDescending(x => x.Blogs.Count(b => !b.IsDeleted))
                    .ThenBy(x => x.Name)
            };
        }

        private async Task<List<SelectListItem>> GetCountryOptionsAsync(int? selectedCountryId = null)
        {
            var countries = await _context.Countries
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = selectedCountryId.HasValue && x.Id == selectedCountryId.Value
                })
                .ToListAsync();

            return countries;
        }
    }
}