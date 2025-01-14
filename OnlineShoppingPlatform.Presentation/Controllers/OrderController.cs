using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess;
using Microsoft.AspNetCore.Authorization;
using OnlineShoppingPlatform.Presentation.Models.Order;
using System.Security.Claims;
using OnlineShoppingPlatform.Business.Operations.Product;
using System.Runtime.CompilerServices;
using OnlineShoppingPlatform.Business.Types;
using OnlineShoppingPlatform.Business.Operations.Order;
using OnlineShoppingPlatform.Business.Operations.Order.Dtos;

namespace OnlineShoppingPlatform.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;


        public OrderController(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }


        // Sipariş oluşturma
        [Authorize]
        [HttpPost("addorder")]
        public async Task<IActionResult> AddOrder([FromBody] AddOrderRequest orderRequest)
        {
            // Model doğrulama: Eksik veya hatalı veriler kontrol edilir
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Gelen sipariş isteği null kontrolü
            if (orderRequest == null)
            {
                return BadRequest("Sipariş veya ürün bilgileri eksik.");
            }

            // Toplam tutarı hesaplamak için değişken
            decimal totalAmount = 0;

            // Sipariş ürünleri listesi oluşturuluyor
            var orderProducts = new List<OrderProduct>();

            // Sipariş içindeki her bir ürünü işliyoruz
            foreach (var orderProductRequest in orderRequest.OrderProducts)
            {
                // Ürünü veritabanından getiriyoruz
                var product = await _productService.GetProductByIdAsync(orderProductRequest.ProductId);

                // Ürün kontrolü: Ürün bulunamazsa hata döndür
                if (product == null)
                {
                    return BadRequest($"Ürün bulunamadı: {orderProductRequest.ProductId}");
                }

                // Ürün fiyatı yoksa 0 kabul edilir
                var unitPrice = (int)(product?.Price ?? 0);

                // Toplam tutarı güncelle
                totalAmount += unitPrice * orderProductRequest.Quantity;

                // Sipariş ürününü oluştur ve listeye ekle
                orderProducts.Add(new OrderProduct
                {
                    ProductId = orderProductRequest.ProductId, // Ürün kimliği
                    Quantity = orderProductRequest.Quantity,   // Sipariş miktarı
                    UnitPrice = unitPrice                      // Ürün fiyatı
                });
            }

            // Sipariş DTO'su oluşturuluyor
            var orderDto = new AddOrderDto
            {
                CustomerId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value ?? "0"), // Kullanıcı kimliği
                TotalAmount = totalAmount,    // Toplam sipariş tutarı
                OrderStatus = orderRequest.OrderStatus // Sipariş durumu
            };

            // Siparişi eklemek için servis çağrısı yapılıyor
            var result = await _orderService.AddOrderAsync(orderDto, orderProducts);

            // İşlem sonucu kontrol ediliyor
            if (result.IsSucceed)
            {
                return Ok(result.Message); // Başarılı ise mesaj döndür
            }
            else
            {
                return BadRequest(result.Message); // Başarısız ise hata mesajı döndür
            }
        }



        // Siparişi Id ile getirme
        [Authorize]
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var result = await _orderService.GetOrderByIdAsync(orderId);

            if (result == null)
            {
                // Sipariş bulunamadı
                return NotFound("The order with the specified ID does not exist.");
            }

            return Ok(result);
        }

        // Siparişleri listeleme
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found.");
            }

            return Ok(orders);
        }

        // Sipariş güncelleme
        [HttpPut("update/{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] UpdateOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // UpdateOrderAsync metodunu çağırıyoruz

            var result = await _orderService.UpdateOrderAsync(orderId, request.UpdatedOrder, request.UpdatedOrderProducts);

            // Eğer güncelleme başarılıysa
            if (result.IsSucceed)
            {
                return Ok(result.Message);  // Başarı mesajı döndürüyoruz
            }
            else
            {
                return BadRequest(result.Message);  // Hata mesajını döndürüyoruz
            }
        }
    

        // Sipariş silme
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            // DeleteOrderAsync metodunu çağırıyoruz
            var result = await _orderService.DeleteOrderAsync(orderId);

            // Eğer silme başarılıysa
            if (result.IsSucceed)
            {
                return Ok(result.Message);  // Başarı mesajı döndürüyoruz
            }
            else
            {
                return BadRequest(result.Message);  // Hata mesajını döndürüyoruz
            }
        }
    }
}
