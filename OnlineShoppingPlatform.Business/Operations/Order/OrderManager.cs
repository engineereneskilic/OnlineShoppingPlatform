using OnlineShoppingPlatform.DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess.Repositories;
using OrderEntity = OnlineShoppingPlatform.DataAccess.Entities.Order;
using UserEntity = OnlineShoppingPlatform.DataAccess.Entities.User;
using ProductEntity = OnlineShoppingPlatform.DataAccess.Entities.Product;
using System.Runtime.CompilerServices;
using OnlineShoppingPlatform.Business.Types;
using OnlineShoppingPlatform.Business.Operations.Order.Dtos;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.Business.Operations.Product;


namespace OnlineShoppingPlatform.Business.Operations.Order
{
    public class OrderManager : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<OrderEntity> _repository;
        private readonly IRepository<OrderProduct> _orderproductRepository;

        private readonly IRepository<UserEntity> _userRepository;
        private readonly IRepository<ProductEntity> _productRepository;

        public OrderManager(IUnitOfWork unitOfWork, IRepository<OrderEntity> repository, IRepository<OrderProduct> orderproductRepository, IRepository<UserEntity> userRepository, IRepository<ProductEntity> productRepository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _orderproductRepository = orderproductRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        // ID'ye göre siparişi getirir
        public async Task<OrderEntity> GetOrderByIdAsync(int orderId)
        {
            // Sipariş ve ilişkili ürünleri veritabanından alıyoruz
            //var order = await _repository.GetByIdAsync(orderId);

            


            // Siparişi ve ilişkili ürünleri almak için asenkron sorgu
            var order = await _repository.GetByQueryAsync(o => o.OrderId == orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found");

            // İlişkili ürünleri dahil etmek için Include ve ThenInclude kullanıyoruz
            var orderWithProducts = order.Include(o => o.OrderProducts)
                                         .ThenInclude(op => op.Product) // Ürün bilgilerini dahil ediyoruz
                                         .FirstOrDefault();  // İlk eşleşeni alıyoruz
            if (orderWithProducts == null)
                throw new KeyNotFoundException("Order with Products not found");

            return orderWithProducts;
        }

        // Tüm siparişleri getirir
        public async Task<List<OrderEntity>> GetAllOrdersAsync()
        {
            // Tüm siparişleri asenkron olarak alın
            var orders = await _repository.GetAllAsync();

            if (orders == null || !orders.Any())
                throw new KeyNotFoundException("No orders found.");

            // İlişkili ürünleri dahil etmek için Include ve ThenInclude kullanıyoruz
            var orderWithProducts = orders.Include(o => o.OrderProducts)
                                         .ThenInclude(op => op.Product) // Ürün bilgilerini dahil ediyoruz
                                         .ToList(); // İlk eşleşeni alıyoruz
            if (orderWithProducts == null)
                throw new KeyNotFoundException("Order with Products not found");


            return orderWithProducts;
        }

        // Yeni bir sipariş oluşturur
        public async Task<ServiceMessage> AddOrderAsync(AddOrderDto addOrderDto, List<OrderProduct> orderProducts)
        {

            if (addOrderDto == null || orderProducts == null || !orderProducts.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Sipariş bilgileri veya sipariş ürünleri eksik."
                };
            }

            await _unitOfWork.BeginTransaction();

            var CurrentUser = await _userRepository.GetByIdAsync(addOrderDto.CustomerId);
            //var CurrentOrderId = await _repository.GetTotalCountsAsync() + 1;

            var newOrder = new OrderEntity
            {
                CustomerId = addOrderDto.CustomerId,
                TotalAmount = addOrderDto.TotalAmount,
                OrderStatus = addOrderDto.OrderStatus,
                User = CurrentUser
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

           // var CurrentOrderProductID = await _repository.GetTotalCountsAsync() + 1;

            // Sipariş ürünlerini ekle
            foreach (var product in orderProducts)
            {
                product.OrderId = newOrder.OrderId;
                product.OrderProductId = newOrder.OrderId;


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
        public async Task<ServiceMessage> UpdateOrderAsync(int orderId, UpdateOrderDto updatedOrderDto, List<OrderProduct> updatedOrderProducts)
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
            var CurrentUser = await _userRepository.GetByIdAsync(updatedOrderDto.CustomerId);

            // 2. Sipariş bilgilerini güncelle
            existingOrder.CustomerId = updatedOrderDto.CustomerId;
            existingOrder.User = CurrentUser;
            existingOrder.OrderDate = updatedOrderDto.OrderDate;
            existingOrder.OrderStatus = updatedOrderDto.OrderStatus;

           


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
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = $"{updatedProduct.ProductId} id numaralı ürün bulunmuyor, o nedenle bu siparişe ekleyemezsiniz."
                    };

                    // Eğer ürün mevcut değilse, yeni bir ürün ekleyelim
                    //existingOrder.OrderProducts.Add(updatedProduct);
                }
            }



            


            // 4. Güncellenen siparişi ve ürünleri kaydet
            try
            {
                await _unitOfWork.BeginTransaction();

                await _unitOfWork.DbSaveChangesAsync();  // Değişiklikleri kaydet

                // Total Amount Güncelle
                existingOrder.TotalAmount = await CheckTotalAmountforOrderById(orderId); 

                await _unitOfWork.DbSaveChangesAsync();  // Değişiklikleri kaydet

                await _unitOfWork.CommitTransaction();

            }
            catch (Exception)
            {

                await _unitOfWork.RollBackTransaction();
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

        public async Task<int> GetTotalCountOrders()
        {
            return await _repository.GetTotalCountsAsync();
        }

        public async Task<decimal> CheckTotalAmountforOrderById(int orderId)
        {
            // Total Amount Hesabı - Tüm ürünler en baştan ele alınır ve totalamount hesaplanır
            var existingOrderwithProducts = await GetOrderByIdAsync(orderId);

            decimal totalAmount = 0;

            if (existingOrderwithProducts.OrderProducts != null)
            {
                foreach (var product in existingOrderwithProducts.OrderProducts)
                {

                    // Ürünü veritabanından getiriyoruz
                    var existProduct = await _productRepository.GetByIdAsync(product.ProductId);

                    // Ürün kontrolü: Ürünün bulunmama ihtimali yok elimizde ürün mutkala var listeden geliyor hiç yoksa buraya gelmez
                    //if (existProduct == null)
                    //{
                    //    return new ServiceMessage
                    //    {
                    //        IsSucceed = false,
                    //        Message = $"{product.Product} id numaralı ürün bulunmuyor"
                    //    };
                    //}

                    // Ürün fiyatı yoksa 0 kabul edilir
                    var unitPrice = (int)(existProduct?.Price ?? 0);

                    // Toplam tutarı güncelle
                    totalAmount += unitPrice * product.Quantity;
                }

                //existingOrder.TotalAmount = totalAmount;
                
            }
            return totalAmount;
        } 
    }
}
