using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShoppingPlatform.DataAccess.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities
{
 

    public class User : BaseEntity
    {
        
        [Key]
        public int Id { get; set; } // Primary Key

        [Required(ErrorMessage = "User name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string UserName { get; set; } = string.Empty; // Kullanıcının adı

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = string.Empty; // Kullanıcının adı

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public  string LastName { get; set; } = string.Empty; // Kullanıcının soyadı

        // Override the Email property with validation attributes
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty; // Telefon numarası

        [Required(ErrorMessage = "Password is required.")]
        // 100 karakter şifre en fazla uzunluğunda olmalı fakat veritabanında şifreli ve uzun bir şekilde kaydedilecek
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string Password { get; set; } = string.Empty; // Default value // Şifre

        [Required(ErrorMessage = "BirthDate is required.")]
        [Range(typeof(DateTime), "1/1/1900", "12/31/2025", ErrorMessage = "BirthDate must be between 01/01/1900 and 12/31/2025.")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Usertype is required.")]
        public string UserType { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } // Kullanıcı rolü

        // Navigation Property
        public ICollection<Order>? Orders { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }



}
