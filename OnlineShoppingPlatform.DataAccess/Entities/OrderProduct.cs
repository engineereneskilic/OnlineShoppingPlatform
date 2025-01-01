using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities
{
    public class OrderProduct
    {
        public int OrderProductId { get; set; }

        //[Key, Column(Order = 0)]
        [Required(ErrorMessage = "Order ID is required.")]
        public int OrderId { get; set; } // Foreign Key

        //[Key, Column(Order = 1)]
        [Required(ErrorMessage = "Product ID is required.")]
        public int ProductId { get; set; } // Foreign Key

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } // Ürün adedi

        // Navigation Properties
        public Order? Order { get; set; } // Sipariş
        public Product? Product { get; set; } // Ürün
    }
}
