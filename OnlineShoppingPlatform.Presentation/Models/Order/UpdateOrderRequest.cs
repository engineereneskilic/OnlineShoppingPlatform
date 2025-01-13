using System.ComponentModel.DataAnnotations;
using OnlineShoppingPlatform.DataAccess.Entities;

namespace OnlineShoppingPlatform.Presentation.Models.Order
{
    public class UpdateOrderRequest
    {
        [Required]
        public DataAccess.Entities.Order UpdatedOrder { get; set; } = new DataAccess.Entities.Order();
        [Required]
        public List<OrderProduct> UpdatedOrderProducts { get; set; } = new List<OrderProduct>();
    }
}
