using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities
{
    public class Product : BaseEntity
    {
        [Key]
        public int ProductId { get; set; } // Primary Key
        public string ProductName { get; set; } = string.Empty;  // Ürün adı
        public decimal Price { get; set; } // Fiyat
        public int StockQuantity { get; set; } // Stok miktarı


        // Navigation Property
        public ICollection<OrderProduct>? OrderProducts { get; set; }
    }

    public class ProductConfiguration : BaseConfigiration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

        }
    }
}
