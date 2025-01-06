using OnlineShoppingPlatform.DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;


        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return (await _unitOfWork.Repository<Product>().GetAllAsync()).ToList();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.DbSaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            await _unitOfWork.Repository<Product>().UpdateAsync(product);
            await _unitOfWork.DbSaveChangesAsync();
            return product;
        }

        public async Task DeleteProductAsync(int id)
        {
            await _unitOfWork.Repository<Product>().DeleteAsync(id);
            await _unitOfWork.DbSaveChangesAsync();
        }
    }
}
