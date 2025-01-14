using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShoppingPlatform.DataAccess.Entities.Enums;

namespace OnlineShoppingPlatform.Business.Operations.Order.Dtos
{
    public class OrderInfoDto
    {
        public int OrderId { get; set; } // Sipariş ID
        public DateTime OrderDate { get; set; } // Sipariş tarihi

        public OrderStatus OrderStatus { get; set; }

        public List<OrderProductInfoDto> Products { get; set; } // Ürün listesi

        public decimal TotalAmount { get; set; } // Toplam tutar

        

    }
}
