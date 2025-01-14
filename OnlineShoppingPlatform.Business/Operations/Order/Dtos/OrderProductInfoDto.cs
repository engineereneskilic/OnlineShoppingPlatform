using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.Business.Operations.Order.Dtos
{
    public class OrderProductInfoDto
    {
        public int ProductId { get; set; } // Ürün ID
        public string ProductName { get; set; } // Ürün adı
        public decimal UnitPrice { get; set; } // Birim fiyat
        public int Quantity { get; set; } // Miktar
    }
}
