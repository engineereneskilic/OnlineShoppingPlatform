using System.ComponentModel.DataAnnotations;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess.Entities.Enums;

namespace OnlineShoppingPlatform.Presentation.Models.Order
{
    public class AddOrderRequest
    {


        //[Required(ErrorMessage = "Total amount is required.")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0.")]
        //public decimal TotalAmount { get; set; } // Toplam tutar kullanıcıdan almıyoruz kendimiz hesaplıyoruz

        [Required]
        public OrderStatus OrderStatus { get; set; }

        [Required]
        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>(); // Siparişin ürünleri


    }
}
