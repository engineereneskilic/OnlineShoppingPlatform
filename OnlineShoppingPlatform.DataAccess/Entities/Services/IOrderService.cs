using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order, List<OrderProduct> orderProducts);
        Task UpdateOrderAsync(int orderId, Order updatedOrder, List<OrderProduct> updatedOrderProducts);
        Task DeleteOrderAsync(int orderId);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
    }
}
