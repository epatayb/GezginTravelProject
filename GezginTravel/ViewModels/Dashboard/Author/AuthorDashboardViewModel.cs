namespace GezginTravel.ViewModels.Dashboard.Author
{
    public class AuthorDashboardViewModel
    {
        public int MyBlogCount { get; set; }
        public int TotalViews { get; set; }
        public int TotalLikes { get; set; }
        public int TotalComments { get; set; }
        public int TotalSaves { get; set; }

        public decimal AverageTrendScore { get; set; }

        public string? TopBlogTitle { get; set; }
        public decimal TopBlogTrendScore { get; set; }

        public string? MostViewedBlogTitle { get; set; }
        public int MostViewedBlogCount { get; set; }
    }
}
