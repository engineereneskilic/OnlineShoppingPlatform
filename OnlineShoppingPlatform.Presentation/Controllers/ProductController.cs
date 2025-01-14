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
using System.Reflection.Metadata.Ecma335;
using OnlineShoppingPlatform.Business.Types;

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
        //[Authorize(Roles = "Admin")]
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
        //[Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductInfoDto>> GetProductById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Geçersiz ürün ID'si."
                });
            }

            var product = await _productService.GetProductByIdAsync(id);

            var productInfoDto = new ProductInfoDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };

            return Ok(productInfoDto);
        }

        // Ürünleri listeleme
        //[Authorize]
        [HttpGet("all")]
        public async Task<ActionResult<List<ProductInfoDto>>> GetAllProducts()
        {

            var productList= await _productService.GetAllProductsAsync();

            var productInfoDtoList = productList.Select(product => new ProductInfoDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price,
                StockQuantity = product.StockQuantity

            }).ToList();


            return Ok(productInfoDtoList!);

        }

        // Ürün güncelleme
        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest productRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Geçersiz ürün ID'si."
                });
            }


            var updateProductDto = new UpdateProductDto
            {
                ProductId = id,
                ProductName = productRequest.ProductName,
                Price = productRequest.Price,
                StockQuantity = productRequest.StockQuantity
            };


            if (updateProductDto == null)
            {
                return BadRequest("Product data cannot be null.");
            }
            
            var result = await _productService.UpdateProductAsync(updateProductDto);

            if (result.IsSucceed)
            {
                return Ok(result.Message);
            }
 
            return BadRequest(result.Message);
     
        }

        // Ürün silme
        //[Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Geçersiz ürün ID'si."
                });
            }

            var result = await _productService.DeleteProductAsync(id);

            if (result.IsSucceed)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);        
        }
    }
}
