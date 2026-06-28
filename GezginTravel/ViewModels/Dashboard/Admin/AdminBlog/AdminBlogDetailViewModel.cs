namespace GezginTravel.ViewModels.Dashboard.Admin.AdminBlog
{
    public class AdminBlogDetailViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;

        public int EstimatedReadingTime { get; set; }

        public string AuthorName { get; set; } = string.Empty;
        public string AuthorEmail { get; set; } = string.Empty;
        public string AuthorTitle { get; set; } = string.Empty;
        public string AuthorPhotoUrl { get; set; } = string.Empty;

        public string CityName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;

        public List<string> Categories { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public List<string> Images { get; set; } = new();

        public int ViewCount { get; set; }
        public int BlogViewRecordCount { get; set; }
        public int UniqueIpCount { get; set; }
        public int RegisteredViewCount { get; set; }
        public int AnonymousViewCount { get; set; }

        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int SaveCount { get; set; }

        public int InteractionScore { get; set; }
        public decimal TrendScore { get; set; }

        public string StatusText { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }

        public DateTime? LastViewedAt { get; set; }
        public DateTime? LastCommentedAt { get; set; }

        public List<AdminBlogCommentItemViewModel> RecentComments { get; set; } = new();
        public List<AdminBlogViewItemViewModel> RecentViews { get; set; } = new();
    }

    public class AdminBlogCommentItemViewModel
    {
        public string UserFullName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class AdminBlogViewItemViewModel
    {
        public string ViewerName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

}
