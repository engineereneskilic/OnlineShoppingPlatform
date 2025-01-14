using OnlineShoppingPlatform.DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess.Repositories;
using OrderEntity = OnlineShoppingPlatform.DataAccess.Entities.Order;
using System.Runtime.CompilerServices;
using OnlineShoppingPlatform.Business.Types;
using OnlineShoppingPlatform.Business.Operations.Order.Dtos;
using Microsoft.EntityFrameworkCore;


namespace OnlineShoppingPlatform.Business.Operations.Order
{
    public class OrderManager : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<OrderEntity> _repository;
        private readonly IRepository<OrderProduct> _orderproductRepository;

        public OrderManager(IUnitOfWork unitOfWork, IRepository<OrderEntity> repository, IRepository<OrderProduct> orderproductRepository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _orderproductRepository = orderproductRepository;
        }

        // ID'ye göre siparişi getirir
        public async Task<OrderEntity> GetOrderByIdAsync(int orderId)
        {
            // Siparişi ve ilişkili ürünleri almak için asenkron sorgu
            var order = await _repository.GetByQueryAsync(o => o.OrderId == orderId);

            // İlişkili ürünleri dahil etmek için Include ve ThenInclude kullanıyoruz
            var orderWithProducts = order.Include(o => o.OrderProducts)
                                         .ThenInclude(op => op.Product) // Ürün bilgilerini dahil ediyoruz
                                         .FirstOrDefault();  // İlk eşleşeni alıyoruz

            return orderWithProducts!;
        }

        // Tüm siparişleri getirir
        public async Task<List<OrderEntity>> GetAllOrdersAsync()
        {
            // Tüm siparişleri asenkron olarak alın
            var orders = await _repository.GetAllAsync();

            // Include ve ThenInclude kullanarak ilişkili ürünleri dahil edin
            var ordersWithProducts = orders.Include(o => o.OrderProducts)
                                            .ThenInclude(op => op.Product) // Ürün bilgilerini dahil ediyoruz
                                            .ToList();

            return ordersWithProducts;
        }

        // Yeni bir sipariş oluşturur
        public async Task<ServiceMessage> AddOrderAsync(AddOrderDto order, List<OrderProduct> orderProducts)
        {

            if (order == null || orderProducts == null || !orderProducts.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Sipariş bilgileri veya sipariş ürünleri eksik."
                };
            }

            await _unitOfWork.BeginTransaction();

            var newOrder = new OrderEntity
            {
                CustomerId = order.CustomerId,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus
            };
           
            try
            {
                // Siparişi ekle
                await _repository.AddAsync(newOrder);
            }
            catch (Exception)
            {

                throw new Exception("Sipariş kaydı sırasında bir sorunla karşılaşıldı");
            }

            // Sipariş ürünlerini ekle
            foreach (var product in orderProducts)
            {
                product.OrderId = newOrder.OrderId;
                await _orderproductRepository.AddAsync(product);
            }

            try
            {
            

                // Değişiklikleri kaydet
                await _unitOfWork.DbSaveChangesAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction();
                throw new Exception("Sipariş eklenirken bir hata oluştu: " + ex.Message);
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Sipariş başarıyla eklendi."
            };
        }

        // Siparişi günceller
        public async Task<ServiceMessage> UpdateOrderAsync(int orderId, OrderEntity updatedOrder, List<OrderProduct> updatedOrderProducts)
        {
            // 1. Siparişin mevcut olup olmadığını kontrol et
            var existingOrder = await _repository.GetByIdAsync(orderId);

            if (existingOrder == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "The order with the specified ID does not exist."
                };
            }

            // 2. Sipariş bilgilerini güncelle
            existingOrder.CustomerId = updatedOrder.CustomerId;
            existingOrder.OrderDate = updatedOrder.OrderDate;
            existingOrder.OrderStatus = updatedOrder.OrderStatus;

            // 3. Sipariş ürünlerini güncelle
            foreach (var updatedProduct in updatedOrderProducts)
            {
                var existingOrderProduct = existingOrder.OrderProducts.FirstOrDefault(op => op.ProductId == updatedProduct.ProductId);

                if (existingOrderProduct != null)
                {
                    // Mevcut ürün varsa, quantity ve price'ı güncelle
                    existingOrderProduct.Quantity = updatedProduct.Quantity;
                    existingOrderProduct.UnitPrice = updatedProduct.UnitPrice;
                }
                else
                {
                    // Eğer ürün mevcut değilse, yeni bir ürün ekleyelim
                    existingOrder.OrderProducts.Add(updatedProduct);
                }
            }

            // 4. Güncellenen siparişi ve ürünleri kaydet
            try
            {
                await _unitOfWork.DbSaveChangesAsync();  // Değişiklikleri kaydet
            }
            catch (Exception)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Siparişi güncellerken bir hata oluştu."
                };
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Sipariş başarıyla güncellendi"
            };
        }


        // Siparişi siler
        public async Task<ServiceMessage> DeleteOrderAsync(int orderId)
        {
            // 1. Siparişin mevcut olup olmadığını kontrol et
            var existingOrder = await _repository.GetByIdAsync(orderId);

            if (existingOrder == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu id numaralı ürün bulunumadı."
                };
            }

            // 2. Siparişi sil
            try
            {
                await _repository.DeleteAsync(existingOrder);  // Siparişi silmek için repository'yi çağırıyoruz
                await _unitOfWork.DbSaveChangesAsync();  // Veritabanındaki değişiklikleri kaydet
            }
            catch (Exception)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Siparişi silerken bir hata oluştu"
                };
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Sipariş başarıyla silindi"
            };
        }


    }
}
