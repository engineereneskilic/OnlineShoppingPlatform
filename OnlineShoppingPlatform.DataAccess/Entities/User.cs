using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities
{
    public enum UserRole
    {
        Admin,
        Customer
    }

    public class User : IdentityUser
    {
        //[Key]
        //public override string Id { get; set; } // Primary Key

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = string.Empty; // Kullanıcının adı

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public  string LastName { get; set; } = string.Empty; // Kullanıcının soyadı

        // Override the Email property with validation attributes
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public override string? Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public override string? PhoneNumber { get; set; } = string.Empty; // Telefon numarası

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string Password { get; set; } = string.Empty; // Default value // Şifre

        [Required]
        public UserRole Role { get; set; } // Kullanıcı rolü

        // Navigation Property
        public ICollection<Order>? Orders { get; set; }
    }

}
