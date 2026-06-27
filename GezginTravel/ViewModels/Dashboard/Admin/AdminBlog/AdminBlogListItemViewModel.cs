namespace GezginTravel.ViewModels.Dashboard.Admin.AdminBlog
{
    public class AdminBlogListItemViewModel
    {
        public int Id { get; set; }

        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public string AuthorName { get; set; } = string.Empty;

        public List<string> Categories { get; set; } = new();
        public List<string> Tags { get; set; } = new();

        public string CityName {  get; set; } = string.Empty;
        public string CountryName {  get; set; } = string.Empty;

        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int SaveCount { get; set; }

        public decimal TrendScore { get; set; }

        public DateTime CreatedDate {  get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string Status { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
