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
        [HttpPost]
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
                CustomerId = int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value, out var id) && id > 0 ? id : 1, // Kullanıcı kimliği
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
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderInfoDto>> GetOrderById(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                // Sipariş bulunamadı
                return NotFound("The order with the specified ID does not exist.");
            }

            // DTO'ya dönüştürüyoruz
            var orderInfoDto = new OrderInfoDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,

                Products = order.OrderProducts
                        .Where(op => op.Product != null) // null olan ürünleri filtrele
                .Select(op => new OrderProductInfoDto
                {
                    ProductId = op.Product!.ProductId, // '!' ile null olmadığını belirtiyoruz
                    ProductName = op.Product.ProductName ?? "Unknown Product", // null kontrol
                    UnitPrice = op.Product.Price,
                    Quantity = op.Quantity
                }).ToList(),
                TotalAmount = order.TotalAmount

            };

            return Ok(orderInfoDto);
        }

        // Siparişleri listeleme
        [HttpGet("all")]
        public async Task<ActionResult<List<OrderInfoDto>>> GetAllOrders()
        {
            // Tüm siparişleri veritabanından çekiyoruz
            var orders = await _orderService.GetAllOrdersAsync();

            

            // Siparişleri DTO'ya dönüştürüyoruz
            var orderInfoDtos = orders.Select(order => new OrderInfoDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                Products = order.OrderProducts
                    .Where(op => op.Product != null) // null olan ürünleri filtrele
                    .Select(op => new OrderProductInfoDto
                    {
                        ProductId = op.Product!.ProductId,
                        ProductName = op.Product.ProductName ?? "Unknown Product",
                        UnitPrice = op.Product.Price,
                        Quantity = op.Quantity
                    }).ToList(),
                TotalAmount = order.TotalAmount
            }).ToList();

            return Ok(orderInfoDtos);
        }

        // Sipariş güncelleme
        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] UpdateOrderRequest orderRequest)
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

            // Güncellenen siparişin veritabanında mevcut olup olmadığını kontrol et - ana kutu için
            var existingOrder = await _orderService.GetOrderByIdAsync(orderId);
            if (existingOrder == null)
            {
                return NotFound($"Sipariş bulunamadı: {orderRequest.OrderId}");
            }

            // Toplam tutarı hesaplamak için değişken
            decimal totalAmount = 0;

            // Sipariş ürünleri listesi oluşturuluyor
            var updatedOrderProducts = new List<OrderProduct>();

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

                // Güncellenen sipariş ürününü oluştur ve listeye ekle
                updatedOrderProducts.Add(new OrderProduct
                {
                    ProductId = orderProductRequest.ProductId, // Ürün kimliği
                    Quantity = orderProductRequest.Quantity,   // Sipariş miktarı
                    UnitPrice = unitPrice                      // Ürün fiyatı
                });
            }

            // Güncellenen sipariş DTO'su oluşturuluyor
            var updatedOrderDto = new UpdateOrderDto
            {
                OrderId = orderRequest.OrderId,             // Sipariş kimliği
                CustomerId = existingOrder.CustomerId,      // Müşteri kimliği
                TotalAmount = totalAmount,                  // Toplam sipariş tutarı
                OrderStatus = orderRequest.OrderStatus      // Sipariş durumu
            };

            // Siparişi güncellemek için servis çağrısı yapılıyor
            var result = await _orderService.UpdateOrderAsync(orderId, updatedOrderDto, updatedOrderProducts);

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
