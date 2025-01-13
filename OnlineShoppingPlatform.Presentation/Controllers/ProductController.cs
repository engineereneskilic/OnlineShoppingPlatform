using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess;
using OnlineShoppingPlatform.Presentation.Filters;
using OnlineShoppingPlatform.Business.Operations.Product;
using OnlineShoppingPlatform.Presentation.Models.Product;
using OnlineShoppingPlatform.Business.Operations.Product.Dtos;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductRequest productRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
            } 
           
           return BadRequest(result.Message);

        }

        // Ürün Id ile getirme
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
 
            var product = await _productService.GetProductByIdAsync(id);
            if(product == null)
            {
                return NotFound();
            }

            return product;
        }

        // Ürünleri listeleme
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            return await _productService.GetAllProductsAsync();
        }

        // Ürün güncelleme
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest productRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updateProductDto = new UpdateProductDto
            {
                ProductId = id,
                ProductName = productRequest.ProductName,
                Price = productRequest.Price,
                StockQuantity = productRequest.StockQuantity
            };

            // Kontrol 1: Gönderilen ID ile ürünün ID'si eşleşiyor mu?
            if (id != productRequest.ProductId)
            {
                return BadRequest("The provided ID does not match the product ID.");
            }

            // Kontrol 2: Gönderilen ürün verisi null mı?
            if (updateProductDto == null)
            {
                return BadRequest("Product data cannot be null.");
            }

            // Kontrol 3: Veritabanında ürünün mevcut olup olmadığını kontrol et
                
            var existingProduct = await _productService.GetProductByIdAsync(id);
                
            if (existingProduct == null)    
            {
                return NotFound("The product with the specified ID does not exist.");
            }
            
            var result = await _productService.UpdateProductAsync(updateProductDto);

            if (result.IsSucceed)
            {
                return Ok(result.Message);
            }
 
            return BadRequest(result.Message);
     
        }

        // Ürün silme
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Kontrol 1: Geçerli bir ID gönderildi mi?
            if (id <= 0)
            {
                return BadRequest("The provided ID is invalid.");
            }

            // Kontrol 2: Veritabanında ürünün mevcut olup olmadığını kontrol et
            var existingProduct = await _productService.GetProductByIdAsync(id);

            if (existingProduct == null)
            {
                return NotFound("The product with the specified ID does not exist.");
            }

            // Silme işlemini gerçekleştir
            var result = await _productService.DeleteProductAsync(id);

            if (result.IsSucceed)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);        
        }
    }
}
