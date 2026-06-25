using GezginTravel.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading;

namespace GezginTravel.ViewModels.Dashboard.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalBlogs { get; set; }
        public int TotalCategories { get; set; }
        public int TotalComments { get; set; }
        public int TotalViews { get; set; }
        public int TotalLikes { get; set; }
        public int TotalSaves { get; set; }
        public int TotalCities { get; set; }
        public int TotalCountries { get; set; }

        // En çok içeriğe sahip şehir
        public string? MostContentCityName { get; set; }
        public string? MostContentCityCountryName { get; set; }
        public int MostContentCityBlogCount { get; set; }

        // En çok içeriğe sahip ülke
        public string? MostContentCountryName { get; set; }
        public int MostContentCountryBlogCount { get; set; }

        // Trend blog
        public string? TopTrendBlogTitle { get; set; }
        public decimal TopTrendBlogScore { get; set; }

        // En çok yorum alan blog
        public string? MostViewedBlogTitle { get; set; }
        public int MostViewedBlogCount { get; set; }

        // En çok kaydedilen blog
        public string? MostSavedBlogTitle { get; set; }
        public int MostSavedBlogCount { get; set; }

        // En çok beğenilen blog
        public string? MostLikedBlogTitle { get; set; }
        public int MostLikedBlogCount { get; set; }

        // En çok yoruma sahip blog
        public string? MostCommentBlogTitle { get; set; }
        public int MostCommentBlogCount { get; set; }

        // Trend yazar
        public string? TopAuthorName { get; set; }
        public string? TopAuthorPhotoUrl {  get; set; }
        public decimal TopAuthorTotalViewed { get; set; }
        public string? TopAuthorDisplayViewed { get; set; }
        public decimal TopAuthorScore { get; set; }

        // En çok kullanılan kategori
        public string? MostUsedCategoryName { get; set; }
        public int MostUsedCategoryCount { get; set; }

        // En çok kullanılan tag
        public string? MostUsedTagName { get; set; }
        public int MostUsedTagCount { get; set; }

        // En son yayınlanan blog
        public List<RecentBlogViewModel> RecentBlogs { get; set; } = new();
        public List<RecentUserViewModel> RecentUsers { get; set; } = new();
    }

    public class RecentBlogViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public decimal TrendScore { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class RecentUserViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleText { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
