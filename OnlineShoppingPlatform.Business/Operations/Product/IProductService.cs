using OnlineShoppingPlatform.Business.Operations.Product.Dtos;
using OnlineShoppingPlatform.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductEntity = OnlineShoppingPlatform.DataAccess.Entities.Product;



namespace OnlineShoppingPlatform.Business.Operations.Product
{
    public interface IProductService
    {
       // Task<bool> AnyProductAsync();

        Task<ProductEntity> GetProductByIdAsync(int id);
        
        Task<List<ProductEntity>> GetAllProductsAsync();
        //Task<List<ProductEntity>> GetByQueryAsync();

        Task<ServiceMessage> AddProductAsync(AddProductDto product);
        Task<ServiceMessage> UpdateProductAsync(UpdateProductDto product);
        Task<ServiceMessage> DeleteProductAsync(int id);
    }
}
