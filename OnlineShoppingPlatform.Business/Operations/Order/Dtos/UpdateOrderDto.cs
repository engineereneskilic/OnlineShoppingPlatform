using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShoppingPlatform.DataAccess.Entities.Enums;

namespace OnlineShoppingPlatform.Business.Operations.Order.Dtos
{
    public class UpdateOrderDto
    {

        public int OrderId { get; set; }

     
        public int CustomerId { get; set; } // Siparişi veren müşteri

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow; // Sipariş tarihi

        public OrderStatus OrderStatus { get; set; }

        public decimal TotalAmount { get; set; } // Toplam tutar
    }
}
