using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatform.Presentation.Models.Product
{
    public class AddProductRequest
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; } = string.Empty;  // Ürün adı

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, 99999, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; } = 10m; // Fiyat

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(1, 100, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; } = 1;// Stok miktarı
    }
}
