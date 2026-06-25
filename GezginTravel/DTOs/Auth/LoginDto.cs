using System.ComponentModel.DataAnnotations;

namespace GezginTravel.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage ="E-Posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage ="Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Şifre alanı zorunludur.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
