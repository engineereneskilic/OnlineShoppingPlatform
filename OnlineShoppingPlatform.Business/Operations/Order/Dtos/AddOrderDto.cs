using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess.Entities.Enums;

namespace OnlineShoppingPlatform.Business.Operations.Order.Dtos
{
    public class AddOrderDto
    {

        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerId { get; set; } // Siparişi veren müşteri

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow; // Sipariş tarihi

        public OrderStatus OrderStatus { get; set; }

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0.")]
        public decimal TotalAmount { get; set; } // Toplam tutar

    }
}
