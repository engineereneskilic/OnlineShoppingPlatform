using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShoppingPlatform.DataAccess.Entities;

namespace OnlineShoppingPlatform.Business.Operations.Order
{
    public interface IOrderService
    {
        Task<DataAccess.Entities.Order> CreateOrderAsync(Order order, List<OrderProduct> orderProducts);
        Task UpdateOrderAsync(int orderId, Order updatedOrder, List<OrderProduct> updatedOrderProducts);
        Task DeleteOrderAsync(int orderId);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
    }
}
