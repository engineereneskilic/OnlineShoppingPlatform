using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

        [Required]
        public DateTime OrderDate { get; set; } // Sipariş tarihi

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0.")]
        public decimal TotalAmount { get; set; } // Toplam tutar

        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerId { get; set; } // Siparişi veren müşteri

        // Navigation Properties
        public User? User { get; set; }// Siparişi veren müşteri
        public ICollection<OrderProduct>? OrderProducts { get; set; } // Siparişin ürünleri
    }

    public class OrderConfiguration : BaseConfigiration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

        }
    }
}
