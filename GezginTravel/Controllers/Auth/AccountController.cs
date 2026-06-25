using GezginTravel.Constants;
using GezginTravel.DTOs.Auth;
using GezginTravel.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GezginTravel.Controllers.Auth
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                user.LastLoginDate = DateTime.Now;
                await _userManager.UpdateAsync(user);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
            return View(model);
        }

        [HttpGet("kayit-ol")]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("kayit-ol")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string photoUrl = "~/images/default-user.png";

            if (model.Photo != null && model.Photo.Length > 0)
            {
                var extension = Path.GetExtension(model.Photo.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Photo", "Sadece jpg,jpeg,png veya webp formatı yükleyebilirsiniz.");
                    return View(model);
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "users");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Photo.CopyToAsync(stream);
                }

                photoUrl = $"uploads/users/{fileName}";
            }

            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
                Title = model.Title,
                About = model.About,
                PhotoUrl = photoUrl,
                CreatedAt = DateTime.Now,
                EmailConfirmed = true,
                IsEmailVerified = true,
                PopularityScore = 0
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleConstants.User);

                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        //[HttpGet("sifremi-unuttum")]
        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}

        //[HttpGet("sifremi-unuttum")]
        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public IActionResult ExternalLogin()
        //{ }

        [HttpPost]
        [Authorize(Roles = RoleConstants.User)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BecomeAuthor()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/account/login");
            }

            var isAuthor = await _userManager.IsInRoleAsync(user, RoleConstants.Author);

            if (!isAuthor)
            {
                await _userManager.AddToRoleAsync(user, RoleConstants.Author);
            }

            await _signInManager.RefreshSignInAsync(user);

            return Redirect("/dashboard/index");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("yetkisiz-erisim")]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
