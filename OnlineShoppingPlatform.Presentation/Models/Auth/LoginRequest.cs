using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatform.Presentation.Models.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        // 100 karakter şifre en fazla uzunluğunda olmalı fakat veritabanında şifreli ve uzun bir şekilde kaydedilecek
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
