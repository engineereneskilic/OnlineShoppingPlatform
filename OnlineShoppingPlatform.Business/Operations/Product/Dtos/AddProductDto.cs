using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShoppingPlatform.DataAccess.Entities.Enums;

namespace OnlineShoppingPlatform.Business.Operations.Product.Dtos
{
    public class AddProductDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; } = string.Empty;  // Ürün adı

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; } // Fiyat

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(0, 20, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; } // Stok miktarı
    }
}
