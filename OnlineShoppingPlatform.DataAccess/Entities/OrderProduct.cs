using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineShoppingPlatform.DataAccess.Entities
{
    public class OrderProduct : BaseEntity
    {
        public int OrderProductId { get; set; }

        //[Key, Column(Order = 0)]
        [Required(ErrorMessage = "Order ID is required.")]
        public int OrderId { get; set; } // Foreign Key

        //[Key, Column(Order = 1)]
        [Required(ErrorMessage = "Product ID is required.")]
        public int ProductId { get; set; } // Foreign Key

        [Required(ErrorMessage = "Quantity is required.")]
        public int Quantity { get; set; } // Ürün adedi

        [Required(ErrorMessage = "Unit price is required.")]

        public decimal UnitPrice { get; set; } 

        // Navigation Properties
        public Order? Order { get; set; } // Sipariş
        public Product? Product { get; set; } // Ürün
    }
    public class OrderProductConfiguration : BaseConfigiration<OrderProduct>
    {
        public override void Configure(EntityTypeBuilder<OrderProduct> builder)
        {
            // ben OrderProductId olsun istiyorum o nedenle bu işlemi yapmadım ama istenirse yapılabilir.
            //builder.Ignore(x => x.OrderProductId); // id propertyisini görmezden geldik tabloya aktrılmayacak
            //builder.HasKey("OrderId","ProductId");
            // composite key oluşturup primary key olarak atadık.
            base.Configure(builder);
        }
    }
}
