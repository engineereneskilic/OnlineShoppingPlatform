using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess;
using OnlineShoppingPlatform.Presentation.Filters;
using OnlineShoppingPlatform.Business.Operations.Product;
using OnlineShoppingPlatform.Presentation.Models.Product;
using OnlineShoppingPlatform.Business.Operations.Product.Dtos;

namespace OnlineShoppingPlatform.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TimeRestrictedAccessFilter("06:00", "23:59")] // 06:00 - 23:59 arası erişim izni
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // Ürün oluşturma
        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductRequest productRequest)
        {
            var addProductDto = new AddProductDto
            {
                ProductName = productRequest.ProductName,
                Price = productRequest.Price,
                StockQuantity = productRequest.StockQuantity
            };

            var result = await _productService.AddProductAsync(addProductDto);

            //return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
            if(result.IsSucceed)
            {
                return Ok(result.Message);
            } else
            {
                return BadRequest(result.Message);
            }
        }

        // Ürün Id ile getirme
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            //var product = await _context.Products.FindAsync(id);
            //if (product == null)
            //{
            //    return NotFound();
            //}
            var product = await _productService.GetProductByIdAsync(id);
            if(product == null)
            {
                return NotFound();
            }

            return product;
        }

        // Ürünleri listeleme
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            return await _productService.GetAllProductsAsync();
        }

        // Ürün güncelleme
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            // Kontrol 1: Gönderilen ID ile ürünün ID'si eşleşiyor mu?
            if (id != product.ProductId)
            {
                return BadRequest("The provided ID does not match the product ID.");
            }

            // Kontrol 2: Gönderilen ürün verisi null mı?
            if (product == null)
            {
                return BadRequest("Product data cannot be null.");
            }

            try
            {
                // Kontrol 3: Veritabanında ürünün mevcut olup olmadığını kontrol et
                var existingProduct = await _productService.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound("The product with the specified ID does not exist.");
                }

                // Güncelleme işlemi
                await _productService.UpdateProductAsync(product);

                return Ok("The product has been successfully updated.");
            }
            catch (Exception ex)
            {
                // Hata durumlarını loglamak için
                // _logger.LogError(ex, "An error occurred while updating the product.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Ürün silme
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
