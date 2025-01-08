using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatform.Presentation.Models.Product
{
    public class UpdateProductRequest
    {
        [Required]
        public int ProductId { get; set; } // Primary Key

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; } = string.Empty;  // Ürün adı

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; } // Fiyat

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; } // Stok miktarı
    }
}
