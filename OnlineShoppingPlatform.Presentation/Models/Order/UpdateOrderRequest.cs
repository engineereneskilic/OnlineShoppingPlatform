using OnlineShoppingPlatform.DataAccess.Entities;

namespace OnlineShoppingPlatform.Presentation.Models.Order
{
    public class UpdateOrderRequest
    {
        public DataAccess.Entities.Order UpdatedOrder { get; set; } = new DataAccess.Entities.Order();
        public List<OrderProduct> UpdatedOrderProducts { get; set; } = new List<OrderProduct>();
    }
}
