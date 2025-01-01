using OnlineShoppingPlatform.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Yeni bir sipariş oluşturur
        public async Task<Order> CreateOrderAsync(Order order, List<OrderProduct> orderProducts)
        {
            // Siparişi ekle
            await _unitOfWork.Repository<Order>().AddAsync(order);

            // Sipariş ürünlerini ekle
            foreach (var orderProduct in orderProducts)
            {
                orderProduct.OrderId = order.OrderId;  // OrderId'yi ata
                await _unitOfWork.Repository<OrderProduct>().AddAsync(orderProduct);
            }

            // Değişiklikleri kaydet
            await _unitOfWork.CommitAsync();
            return order;
        }

        // Siparişi günceller
        public async Task UpdateOrderAsync(int orderId, Order updatedOrder, List<OrderProduct> updatedOrderProducts)
        {
            // Siparişi al
            var existingOrder = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            if (existingOrder == null)
            {
                throw new Exception("Order not found");
            }

            // Sipariş bilgilerini güncelle
            existingOrder.TotalAmount = updatedOrder.TotalAmount;
            existingOrder.OrderDate = updatedOrder.OrderDate;
            existingOrder.CustomerId = updatedOrder.CustomerId;

            // Eski OrderProduct'ları sil
            var existingOrderProducts = await _unitOfWork.Repository<OrderProduct>()
                .GetByQueryAsync(op => op.OrderId == orderId);
            foreach (var orderProduct in existingOrderProducts)
            {
                await _unitOfWork.Repository<OrderProduct>().DeleteAsync(orderProduct);
            }

            // Yeni ürünleri ekle
            foreach (var orderProduct in updatedOrderProducts)
            {
                orderProduct.OrderId = orderId; // Yeni OrderId'yi ata
                await _unitOfWork.Repository<OrderProduct>().AddAsync(orderProduct);
            }

            // Değişiklikleri kaydet
            await _unitOfWork.CommitAsync();
        }

        // Siparişi siler
        public async Task DeleteOrderAsync(int orderId)
        {
            var existingOrder = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            if (existingOrder == null)
            {
                throw new Exception("Order not found");
            }

            // Siparişle ilişkili ürünleri sil
            var existingOrderProducts = await _unitOfWork.Repository<OrderProduct>()
                .GetByQueryAsync(op => op.OrderId == orderId);
            foreach (var orderProduct in existingOrderProducts)
            {
                await _unitOfWork.Repository<OrderProduct>().DeleteAsync(orderProduct);
            }

            // Siparişi sil
            await _unitOfWork.Repository<Order>().DeleteAsync(existingOrder);
            await _unitOfWork.CommitAsync();
        }

        // ID'ye göre siparişi getirir
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
        }

        // Tüm siparişleri getirir
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _unitOfWork.Repository<Order>().GetAllAsync();
        }
    }
}
