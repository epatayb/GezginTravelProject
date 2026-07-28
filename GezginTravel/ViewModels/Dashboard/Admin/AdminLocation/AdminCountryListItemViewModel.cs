namespace GezginTravel.ViewModels.Dashboard.Admin.AdminLocation
{
    public class AdminCountryListItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public int CityCount { get; set; }
        public int BlogCount { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
