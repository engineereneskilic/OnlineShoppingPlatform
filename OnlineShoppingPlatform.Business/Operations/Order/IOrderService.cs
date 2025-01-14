using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShoppingPlatform.Business.Operations.Order.Dtos;
using OnlineShoppingPlatform.Business.Types;
using OnlineShoppingPlatform.DataAccess.Entities;
using OrderEntity = OnlineShoppingPlatform.DataAccess.Entities.Order;

namespace OnlineShoppingPlatform.Business.Operations.Order
{
    public interface IOrderService
    {
        Task<OrderEntity> GetOrderByIdAsync(int orderId);
        Task<List<OrderEntity>> GetAllOrdersAsync();

        Task<ServiceMessage> AddOrderAsync(AddOrderDto order, List<OrderProduct> orderProducts);
        Task<ServiceMessage> UpdateOrderAsync(int orderId, UpdateOrderDto updatedOrderDto, List<OrderProduct> updatedOrderProducts);
        Task<ServiceMessage> DeleteOrderAsync(int orderId);


        Task<int> GetTotalCountOrders();

        Task<decimal> CheckTotalAmountforOrderById(int orderId);





    }   
}
