using System.ComponentModel.DataAnnotations;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess.Entities.Enums;

namespace OnlineShoppingPlatform.Presentation.Models.Order
{
    public class AddOrderRequest
    {
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

    /// <summary>
    /// Sipariş ürünü için istek modeli
    /// </summary>
    public class OrderProductRequest
    {
        /// <summary>
        /// Ürün kimliği
        /// </summary>
        [Required(ErrorMessage = "ProductId is required.")]
        public int ProductId { get; set; }

        /// <summary>
        /// Sipariş miktarı
        /// </summary>
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1,20, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
