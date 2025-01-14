using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using OnlineShoppingPlatform.DataAccess.Entities.Enums;
using OnlineShoppingPlatform.DataAccess.Logging;
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
        public int UserId { get; set; } // Primary Key 

        //[Required(ErrorMessage = "User name is required.")]
        //[StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string UserName { get; set; } = string.Empty; // Kullanıcının adı

        //[Required(ErrorMessage = "First name is required.")]
        //[StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = string.Empty; // Kullanıcının adı

        //[Required(ErrorMessage = "Last name is required.")]
        //[StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public  string LastName { get; set; } = string.Empty; // Kullanıcının soyadı

        //[Required(ErrorMessage = "Email is required.")]
        //[EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Phone number is required.")]
        //[Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty; // Telefon numarası

        //[Required(ErrorMessage = "Password is required.")]
        // 100 karakter şifre en fazla uzunluğunda olmalı fakat veritabanında şifreli ve uzun bir şekilde kaydedilecek
        //[StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string Password { get; set; } = string.Empty; // Default value // Şifre

        //[Required(ErrorMessage = "BirthDate is required.")]
        //[JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime BirthDate { get; set; }

        //[Required(ErrorMessage = "Usertype is required.")]
        public UserRole UserType { get; set; }

        // Navigation Property
        public ICollection<Order>? Orders { get; set; }

        public ICollection<RequestLog>? RequestLogs { get; set; } // Bir kullanıcıya ait işlem kayıtları

        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        //public DateTime? UpdatedAt { get; set; }

    }

    public class UserConfiguration : BaseConfigiration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

        }
    }



}
