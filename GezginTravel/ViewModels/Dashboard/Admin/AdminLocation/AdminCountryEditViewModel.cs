using System.ComponentModel.DataAnnotations;

namespace GezginTravel.ViewModels.Dashboard.Admin.AdminLocation
{
    public class AdminCountryEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ülke adı zorunludur.")]
        [StringLength(30, ErrorMessage = "Ülke adı en fazla 30 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;
    }
}
