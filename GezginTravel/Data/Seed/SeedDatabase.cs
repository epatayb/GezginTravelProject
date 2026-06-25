using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GezginTravel.Models.Identity;
using GezginTravel.Models.Entities;
using GezginTravel.Constants;

namespace GezginTravel.Data.Seed
{
    public static class SeedDatabase
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<GezginDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();

            await context.Database.MigrateAsync();

            await SeedRolesAsync(roleManager);

            var adminUser = await SeedAdminUserAsync(userManager);
            var authorUser = await SeedAuthorUserAsync(userManager);
            var testUser = await SeedUserAsync(userManager);

            await SeedCategoriesAsync(context);
            await SeedTagsAsync(context);
            await SeedLocationsAsync(context);
            await SeedBlogsAsync(context, authorUser.Id);
        }

        // role seed
        private static async Task SeedRolesAsync(RoleManager<AppRole> roleManager)
        {
            var roles = new List<AppRole>
            {
                new AppRole
                {
                    Name = RoleConstants.Admin,
                    FullName = RoleConstants.Admin
                },
                new AppRole
                {
                    Name = RoleConstants.Author,
                    FullName = RoleConstants.Author
                },
                new AppRole
                {
                    Name = RoleConstants.User,
                    FullName = RoleConstants.User
                }
            };

            foreach (var role in roles)
            {
                var exists = await roleManager.RoleExistsAsync(role.Name!);

                if (!exists)
                {
                    var result = await roleManager.CreateAsync(role);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                        throw new Exception($"Rol oluşturulamadı: {role.Name} - {errors}");
                    }
                }
            }
        }

        private static async Task EnsureUserInRoleAsync(UserManager<AppUser> userManager, AppUser user, string roleName)
        {
            var isInRole = await userManager.IsInRoleAsync(user, roleName);

            if (!isInRole)
            {
                var result = await userManager.AddToRoleAsync(user, roleName);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                    throw new Exception($"Kullanıcı role eklenemedi: {user.Email} -> {roleName} - {errors}");
                }
            }
        }

        // admin user
        private static async Task<AppUser> SeedAdminUserAsync(UserManager<AppUser> userManager)
        {
            var adminEmail = "admin@gezgin.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FirstName = "Bünyamin",
                    LastName = "Epatay",
                    Title = "Sistem Yöneticisi",
                    About = "Gezgin Travel platformunun yönetici hesabı.",
                    PhotoUrl = "/uploads/users/admin.jpeg",
                    CreatedAt = DateTime.Now,
                    EmailConfirmed = true,
                    IsEmailVerified = true,
                    PopularityScore = 0
                };

                var result = await userManager.CreateAsync(adminUser, "Admin1453*");

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                    throw new Exception($"Admin kullanıcı oluşturulamadı: {errors}");
                }
            }
            await EnsureUserInRoleAsync(userManager, adminUser, RoleConstants.Admin);
            await EnsureUserInRoleAsync(userManager, adminUser, RoleConstants.Author);
            await EnsureUserInRoleAsync(userManager, adminUser, RoleConstants.User);

            return adminUser;
        }

        // test user
        private static async Task<AppUser> SeedUserAsync(UserManager<AppUser> userManager)
        {

            var userEmail = "user@gezgin.com";
            var user = await userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "testuser",
                    Email = userEmail,
                    FirstName = "test",
                    LastName = "user",
                    Title = "",
                    About = "",
                    PhotoUrl = "/uploads/users/default-user.png",
                    CreatedAt = DateTime.Now,
                    EmailConfirmed = true,
                    IsEmailVerified = true,
                    PopularityScore = 0
                };

                var result = await userManager.CreateAsync(user, "User123.");

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                    throw new Exception($"Test kullanıcı oluşturulamadı: {errors}");
                }
            }
            await EnsureUserInRoleAsync(userManager, user, RoleConstants.User);

            return user;
        }

        // author user
        private static async Task<AppUser> SeedAuthorUserAsync(UserManager<AppUser> userManager)
        {
            var email = "author@gezgin.com";
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "gezginyazar",
                    Email = email,
                    FirstName = "Gezgin",
                    LastName = "Yazar",
                    Title = "Seyahat Yazarı",
                    About = "Yeni şehirler keşfetmeyi ve deneyimlerini paylaşmayı seven bir yazar.",
                    PhotoUrl = "/uploads/users/author.png",
                    CreatedAt = DateTime.Now,
                    EmailConfirmed = true,
                    IsEmailVerified = true,
                    PopularityScore = 0
                };

                var result = await userManager.CreateAsync(user, "Author123*");

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                    throw new Exception($"Yazar kullanıcı oluşturulamadı: {errors}");
                }
            }
            await EnsureUserInRoleAsync(userManager, user, RoleConstants.Author);
            await EnsureUserInRoleAsync(userManager, user, RoleConstants.User);

            return user;
        }

        // category seed
        private static async Task SeedCategoriesAsync(GezginDbContext context)
        {
            if (await context.Categories.AnyAsync())
            {
                return;
            }

            context.Categories.AddRange(
                new Category { Name = "Doğa", Slug = "doga" },
                new Category { Name = "Tarih", Slug = "tarih" },
                new Category { Name = "Kültür", Slug = "kultur" },
                new Category { Name = "Yemek", Slug = "yemek" },
                new Category { Name = "Macera", Slug = "macera" },
                new Category { Name = "Kamp", Slug = "kamp" }
            );
            await context.SaveChangesAsync();
        }

        // tag seed
        private static async Task SeedTagsAsync(GezginDbContext context)
        {
            if (await context.Tags.AnyAsync())
            {
                return;
            }

            context.Tags.AddRange(
                new Tag { Name = "Karadeniz", Slug = "karadeniz" },
                new Tag { Name = "İstanbul", Slug = "istanbul" },
                new Tag { Name = "Yayla", Slug = "yayla" },
                new Tag { Name = "Deniz", Slug = "deniz" },
                new Tag { Name = "Gastronomi", Slug = "gastronomi" },
                new Tag { Name = "Avrupa", Slug = "avrupa" },
                new Tag { Name = "Asya", Slug = "asya" },
                new Tag { Name = "Nature", Slug = "doga" },
                new Tag { Name = "Kumsal", Slug = "kumsal" },
                new Tag { Name = "Orman", Slug = "orman" }
            );
            await context.SaveChangesAsync();
        }

        // location seed
        private static async Task SeedLocationsAsync(GezginDbContext context)
        {
            if (await context.Countries.AnyAsync())
            {
                return;
            }

            var turkiye = new Country { Name = "Türkiye" };
            var fransa = new Country { Name = "Fransa" };

            context.Countries.AddRange(turkiye, fransa);
            await context.SaveChangesAsync();

            context.Cities.AddRange(
                new City { Name = "İstanbul", CountryId = turkiye.Id },
                new City { Name = "Trabzon", CountryId = turkiye.Id },
                new City { Name = "Rize", CountryId = turkiye.Id },
                new City { Name = "Paris", CountryId = fransa.Id }
            );

            await context.SaveChangesAsync();
        }

        // blog seed
        private static async Task SeedBlogsAsync(GezginDbContext context, int authorId)
        {
            if (await context.Blogs.AnyAsync())
            {
                return;
            }

            var turkiye = await context.Countries.FirstAsync(x => x.Name == "Türkiye");
            var fransa = await context.Countries.FirstAsync(x => x.Name == "Fransa");

            var istanbul = await context.Cities.FirstAsync(x => x.Name == "İstanbul");
            var trabzon = await context.Cities.FirstAsync(x => x.Name == "Trabzon");
            var paris = await context.Cities.FirstAsync(x => x.Name == "Paris");

            context.Blogs.AddRange(
                new Blog
                {
                    Title = "İstanbul’da 3 Günlük Gezi Rehberi",
                    Slug = "istanbulda-3-gunluk-gezi-rehberi",
                    Content = "İstanbul, tarihi dokusu, sokak lezzetleri ve boğaz manzarasıyla unutulmaz bir seyahat deneyimi sunar.",
                    ThumbnailUrl = "/images/blogs/istanbul.jpg",
                    EstimatedReadingTime = 5,
                    AuthorId = authorId,
                    CountryId = turkiye.Id,
                    CityId = istanbul.Id,
                    ViewCount = 120,
                    LikeCount = 18,
                    CommentCount = 6,
                    SaveCount = 10,
                    TrendScore = 0
                },
                new Blog
                {
                    Title = "Karadeniz Yaylalarında Bir Hafta",
                    Slug = "karadeniz-yaylalarinda-bir-hafta",
                    Content = "Trabzon ve Rize yaylaları doğa severler için harika rotalar sunar.",
                    ThumbnailUrl = "/images/blogs/karadeniz.jpg",
                    EstimatedReadingTime = 7,
                    AuthorId = authorId,
                    CountryId = turkiye.Id,
                    CityId = trabzon.Id,
                    ViewCount = 200,
                    LikeCount = 32,
                    CommentCount = 11,
                    SaveCount = 20,
                    TrendScore = 0
                },
                new Blog
                {
                    Title = "Paris’te İlk Kez Gezilecek Yerler",
                    Slug = "pariste-ilk-kez-gezilecek-yerler",
                    Content = "Paris, sanat, mimari ve kültür dolu sokaklarıyla Avrupa’nın en özel şehirlerinden biridir.",
                    ThumbnailUrl = "/images/blogs/paris.jpg",
                    EstimatedReadingTime = 6,
                    AuthorId = authorId,
                    CountryId = fransa.Id,
                    CityId = paris.Id,
                    ViewCount = 90,
                    LikeCount = 14,
                    CommentCount = 3,
                    SaveCount = 7,
                    TrendScore = 0
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
