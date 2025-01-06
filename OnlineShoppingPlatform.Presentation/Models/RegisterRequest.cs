using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatform.Presentation.Models
{
    public class RegisterRequest
    {

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }

        public string? PhoneNumber { get; set; } = string.Empty; // Telefon numarası


    }
}
