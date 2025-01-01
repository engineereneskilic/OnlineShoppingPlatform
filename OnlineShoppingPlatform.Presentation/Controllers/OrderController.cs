using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess;

namespace OnlineShoppingPlatform.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // Sipariş oluşturma
        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        // Siparişi Id ile getirme
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // Siparişleri listeleme
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // Sipariş güncelleme
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Sipariş silme
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
