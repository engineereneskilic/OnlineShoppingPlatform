using System.ComponentModel.DataAnnotations;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess.Entities.Enums;

namespace OnlineShoppingPlatform.Presentation.Models.Order
{
    public class UpdateOrderRequest
    {

        [Required(ErrorMessage = "OrderId is required.")]
        public int OrderId { get; set; }

        /// <summary>
        /// Siparişin durumu (ör. Bekliyor, Tamamlandı)
        /// </summary>
        [Required(ErrorMessage = "OrderStatus is required.")]
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// Siparişe dahil olan ürünler
        /// </summary>
        [Required(ErrorMessage = "OrderProducts is required.")]
        public ICollection<OrderProductRequest> OrderProducts { get; set; } = new List<OrderProductRequest>();



    }

 
}
