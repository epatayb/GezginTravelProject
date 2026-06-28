namespace GezginTravel.ViewModels.Dashboard.Admin.AdminCategory
{
    public class AdminCategoryIndexViewModel
    {
        public string? SearchText { get; set; }
        public string Status { get; set; } = "Active";
        public string? SortBy {  get; set; }

        public int TotalCategories { get; set; }
        public int ActiveCategories { get; set; }
        public int DeletedCategories { get; set; }
        public int UsedCategories { get; set; }

        public List<AdminCategoryListItemViewModel> Categories { get; set; } = new();

        public int CurrentPage { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling(TotalItems / (double)PageSize);

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
