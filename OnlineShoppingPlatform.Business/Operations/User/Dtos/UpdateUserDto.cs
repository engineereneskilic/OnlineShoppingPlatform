using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.Business.Operations.User.Dtos
{
    public class UpdateUserDto
    {

        public int UserId { get; set; } // Primary Key 

        public string UserName { get; set; } = string.Empty; // Kullanıcının adı


        public string FirstName { get; set; } = string.Empty; // Kullanıcının adı

        public string LastName { get; set; } = string.Empty; // Kullanıcının soyadı



        public string Email { get; set; } = string.Empty;


        public string Password { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty; // Telefon numarası

        // bunlar otomatik yapılacak kullanıcıdan almicaz
        //[Required(ErrorMessage = "Usertype is required.")]
        //public string UserType { get; set; } = string.Empty;

        //[Required]
        //public UserRole Role { get; set; } // Kullanıcı rolü

        public DateTime BirthDate { get; set; }
    }
}
