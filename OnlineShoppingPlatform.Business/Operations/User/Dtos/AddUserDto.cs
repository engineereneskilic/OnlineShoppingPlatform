using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShoppingPlatform.DataAccess.Entities.Enums;

namespace OnlineShoppingPlatform.Business.Operations.User.Dtos
{
    public class AddUserDto
    {
        [Required(ErrorMessage = "User name is required.")]
        [StringLength(50, ErrorMessage = "User name cannot exceed 50 characters.")]
        public string UserName { get; set; } = string.Empty; // Kullanıcının adı

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = string.Empty; // Kullanıcının adı

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; } = string.Empty; // Kullanıcının soyadı

       
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string Password { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; } = string.Empty; // Telefon numarası

        // bunlar otomatik yapılacak kullanıcıdan almicaz
        //[Required(ErrorMessage = "Usertype is required.")]
        //public string UserType { get; set; } = string.Empty;

        //[Required]
        //public UserRole Role { get; set; } // Kullanıcı rolü

        [Required(ErrorMessage = "BirthDate is required.")]
        [Range(typeof(DateTime), "1/1/1900", "12/31/2025", ErrorMessage = "BirthDate must be between 01/01/1900 and 12/31/2025.")]
        public DateTime BirthDate { get; set; }


    }
}
