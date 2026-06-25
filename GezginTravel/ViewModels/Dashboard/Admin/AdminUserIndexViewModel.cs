namespace GezginTravel.ViewModels.Dashboard.Admin
{
    public class AdminUserIndexViewModel
    {
        public string? SearchText { get; set; }
        public string? SelectedRole { get; set; }

        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalAuthors { get; set; }
        public int SuspendedUsers { get; set; }

        public List<string> Roles { get; set; } = new();
        public List<AdminUserListItemViewModel> Users { get; set; } = new();

        // Pagination
        public int CurrentPage { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling(TotalItems / (double)PageSize);

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
