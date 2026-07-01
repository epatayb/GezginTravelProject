using System.ComponentModel.DataAnnotations;

namespace GezginTravel.ViewModels.Dashboard.Admin.AdminTag
{
    public class AdminTagEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Etiket adı zorunludur.")]
        [StringLength(30, ErrorMessage = "Etiket adı en fazla 30 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;
    }
}
