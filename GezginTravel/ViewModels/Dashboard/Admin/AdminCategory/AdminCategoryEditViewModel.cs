using System.ComponentModel.DataAnnotations;

namespace GezginTravel.ViewModels.Dashboard.Admin.AdminCategory
{
    public class AdminCategoryEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori Adı zorunludur.")]
        [StringLength(25, ErrorMessage = "Kategori Adı en fazla 25 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;
    }
}
