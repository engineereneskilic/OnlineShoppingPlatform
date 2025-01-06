using OnlineShoppingPlatform.Business.Operations.Product.Dtos;
using OnlineShoppingPlatform.Business.Types;
using OnlineShoppingPlatform.DataAccess.Repositories;
using OnlineShoppingPlatform.DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductEntity = OnlineShoppingPlatform.DataAccess.Entities.Product;


namespace OnlineShoppingPlatform.Business.Operations.Product
{
    public class ProductManager : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ProductEntity> _repository; 



        public ProductManager(IUnitOfWork unitOfWork,IRepository<ProductEntity> repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<ProductEntity> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.Repository<ProductEntity>().GetByIdAsync(id);
        }


        public async Task<List<ProductEntity>> GetAllProductsAsync()
        {
            return (await _unitOfWork.Repository<ProductEntity>().GetAllAsync()).ToList();
        }



        public async Task<ServiceMessage> AddProductAsync(AddProductDto addProductDto)
        {

            var HasProduct = await _repository.GetByQueryAsync(x => x.ProductName.Equals(addProductDto.ProductName, StringComparison.OrdinalIgnoreCase));
            
            if(HasProduct.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu ürün zaten var, ekleyemezsiniz."
                };
            }

            var newproduct = new ProductEntity
            {
                ProductName = addProductDto.ProductName,
                StockQuantity = addProductDto.StockQuantity,
                Price = addProductDto.Price
            };

            await _repository.AddAsync(newproduct);

            try
            {
                await _unitOfWork.DbSaveChangesAsync();

            }
            catch (Exception)
            {

                throw new Exception("Ürün kaydı sırasında bir hata oluştu");
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Ürün Başarıyla Eklendi"
            };
        }

        public async Task<ProductEntity> UpdateProductAsync(ProductEntity product)
        {
            await _unitOfWork.Repository<ProductEntity>().UpdateAsync(product);
            await _unitOfWork.DbSaveChangesAsync();
            return product;
        }

        public async Task DeleteProductAsync(int id)
        {
            await _unitOfWork.Repository<ProductEntity>().DeleteAsync(id);
            await _unitOfWork.DbSaveChangesAsync();
        }

    
    }
}
