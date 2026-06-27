using Microsoft.AspNetCore.Mvc.Rendering;

namespace GezginTravel.ViewModels.Dashboard.Admin.AdminBlog
{
    public class AdminBlogIndexViewModel
    {
        public string? SearchText { get; set; }

        public int? SelectedAuthorId { get; set; }
        public int? SelectedCityId { get; set; }
        public int? SelectedCategoryId { get; set; }

        public string? SelectedStatus { get; set; }
        public string? SelectedSortBy { get; set; }

        public List<AdminBlogListItemViewModel> Blogs { get; set; } = new();

        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; } = 10;

        public int TotalPages => 
            (int)Math.Ceiling(TotalItems / (double)PageSize);

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public List<SelectListItem> AuthorOptions { get; internal set; } = new();
        public List<SelectListItem> CityOptions { get; internal set; } = new();
        public List<SelectListItem> CategoryOptions { get; internal set; } = new();
        public List<SelectListItem> StatusOptions { get; internal set; } = new();
        public List<SelectListItem> SortOptions { get; internal set; } = new();
    }
}
