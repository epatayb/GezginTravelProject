using System.ComponentModel.DataAnnotations;

namespace GezginTravel.ViewModels.Dashboard.Admin.AdminLocation
{
    public class AdminCountryCreateViewModel
    {
        [Required(ErrorMessage = "Ülke adı zorunludur.")]
        [StringLength(30, ErrorMessage = "Ülke adı en fazla 30 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;
    }
}
