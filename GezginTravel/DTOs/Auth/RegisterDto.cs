using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GezginTravel.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage ="Ad alanı zorunludur.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        public string Title { get; set; } = string.Empty;

        public string About { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fotoğraf alanı zorunludur.")]
        public IFormFile Photo { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre tekrar alanı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }

        [Range(typeof(bool), "true","true", ErrorMessage = "Devam etmek için kullanım şartlarını kabul etmelisiniz.")]
        public bool AcceptTerms { get; set; }
    }
}
