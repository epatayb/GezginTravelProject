using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GezginTravel.ViewModels.Dashboard.Admin.AdminLocation
{
    public class AdminCityEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Şehir adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Şehir adı en fazla 50 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ülke seçimi zorunludur.")]
        public int? CountryId { get; set; }

        public string Slug { get; set; } = string.Empty;

        public List<SelectListItem> CountryOptions { get; set; } = new();
    }
}
