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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (orderRequest == null || orderRequest.OrderProducts == null || !orderRequest.OrderProducts.Any())
            {
                return BadRequest("Sipariş veya ürün bilgileri eksik.");
            }



            // Toplam tutar ve sipariş ürünleri hazırlanıyor
            decimal totalAmount = 0;
            decimal unitPrice = 0;

            var orderProducts = new List<OrderProduct>();

            foreach (var orderProductRequest in orderRequest.OrderProducts)
            {
                // Ürünü getir
                var product = await _productService.GetProductByIdAsync(orderProductRequest.ProductId);
                if (product == null)
                {
                    return BadRequest($"Ürün bulunamadı: {orderProductRequest.ProductId}");
                }

                // Ürün fiyatını ve miktarını kullanarak toplam tutarı güncelle
                unitPrice = (int)(product?.Price ?? 0); // Ürün fiyatı yoksa 0 kabul edilir
                totalAmount += unitPrice * orderProductRequest.Quantity;

                // Sipariş ürünü oluştur ve listeye ekle
                orderProducts.Add(new OrderProduct
                {
                    ProductId = orderProductRequest.ProductId,
                    Quantity = orderProductRequest.Quantity,
                    UnitPrice = unitPrice
                });
            }

            var orderDto = new AddOrderDto
            {
                CustomerId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value!),
                TotalAmount = totalAmount,
                OrderStatus = orderRequest.OrderStatus
            };

            var result = await _orderService.AddOrderAsync(orderDto, orderProducts);

            if (result.IsSucceed)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
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
