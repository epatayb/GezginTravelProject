namespace GezginTravel.ViewModels.Dashboard.Admin.AdminTag
{
    public class AdminTagIndexViewModel
    {
        public string? SearchText { get; set; }
        public string Status { get; set; } = "Active";
        public string? SortBy {  get; set; }

        public int TotalTags { get; set; }
        public int ActiveTags { get; set; }
        public int DeletedTags { get; set; }
        public int UsedTags { get; set; }

        public List<AdminTagListItemViewModel> Tags { get; set; } = new();

        public int CurrentPage { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling(TotalItems / (double)PageSize);

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
