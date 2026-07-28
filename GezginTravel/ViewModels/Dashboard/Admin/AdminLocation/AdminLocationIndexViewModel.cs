using Microsoft.AspNetCore.Mvc.Rendering;

namespace GezginTravel.ViewModels.Dashboard.Admin.AdminLocation
{
    public class AdminLocationIndexViewModel
    {
        public string? SearchText { get; set; }
        public string Status { get; set; } = "active";
        public string? SortBy { get; set; }

        public int? SelectedCountryId { get; set; }

        public int TotalCountries { get; set; }
        public int ActiveCountries { get; set; }
        public int DeletedCountries { get; set; }

        public string MostCitiesCountryName { get; set; } = "-";
        public int MostCitiesCountryCount { get; set; }

        public int TotalCities { get; set; }
        public int ActiveCities { get; set; }
        public int DeletedCities { get; set; }

        public int UsedCities { get; set; }

        public string TopBlogCityName { get; set; } = "-";
        public int TopBlogCityCount { get; set; }

        public string TopBlogCountryName { get; set; } = "-";
        public int TopBlogCountryCount { get; set; }

        public List<SelectListItem> CountryOptions { get; set; } = new();

        public List<AdminCountryListItemViewModel> Countries { get; set; } = new();
        public List<AdminCityListItemViewModel> Cities { get; set; } = new();
    }
}
