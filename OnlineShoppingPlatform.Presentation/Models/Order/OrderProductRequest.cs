using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatform.Presentation.Models.Order
{
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
        [Range(1, 20, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
