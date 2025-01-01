using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities.Services
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}
