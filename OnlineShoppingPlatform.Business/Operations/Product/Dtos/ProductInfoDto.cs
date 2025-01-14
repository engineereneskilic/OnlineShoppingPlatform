using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.Business.Operations.Product.Dtos
{
    public class ProductInfoDto
    {

        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;  // Ürün adı
        public decimal Price { get; set; } // Fiyat

        public int StockQuantity { get; set; } // Stok miktarı
    }
}
