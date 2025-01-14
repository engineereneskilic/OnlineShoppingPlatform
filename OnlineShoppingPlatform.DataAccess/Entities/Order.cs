using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShoppingPlatform.DataAccess.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities
{
    public class Order : BaseEntity
    {
        [Key]
        public int OrderId { get; set; } // Primary Key
        public DateTime OrderDate { get; set; } // Sipariş tarihi
        public decimal TotalAmount { get; set; } // Toplam tutar
        public OrderStatus OrderStatus { get; set; }



        public int CustomerId { get; set; } // Siparişi veren müşteri

        // Navigation Properties

        public User User { get; set; } = new User(); // Siparişi veren müşteri

        public ICollection<OrderProduct>? OrderProducts { get; set; } = new List<OrderProduct>(); // Siparişin ürünleri
    }

    public class OrderConfiguration : BaseConfigiration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

        }
    }
}
