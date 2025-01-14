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


            // Veritabanında ürünü arayın
            var product = await _repository.GetByIdAsync(id);

            // Eğer ürün bulunmazsa, null kontrolü yapın
            if (product == null)
            {
                throw new KeyNotFoundException($"ID: {id} ile ürün bulunamadı.");
            }

            return product;

            //return await _unitOfWork.Repository<ProductEntity>().GetByIdAsync(id);
        }


        public async Task<List<ProductEntity>> GetAllProductsAsync()
        {
            // Tüm ürünleri veritabanından getir
            var products = (await _repository.GetAllAsync());

            // Eğer hiçbir ürün bulunamazsa, boş bir liste döndür
            if (products == null || !products.Any())
            {
                throw new KeyNotFoundException("Veritabanında hiçbir ürün bulunamadı.");
            }

            return products.ToList();
        }


        
        public async Task<ServiceMessage> AddProductAsync(AddProductDto addProductDto)
        {

            var hasProduct = await _repository.GetByQueryAsync(x =>
                x.ProductName.ToLower() == addProductDto.ProductName.ToLower());

            if (hasProduct.Any())
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

           

            try
            {
                await _repository.AddAsync(newproduct);
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

        public async Task<ServiceMessage> UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            // Güncellenecek ürün veritabanında var mı kontrol et
            var existingProduct = await _repository.GetByIdAsync(updateProductDto.ProductId);
            if (existingProduct == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Güncellemek istediğiniz ürün bulunamadı."
                };
            }

            // Aynı isimde başka bir ürün var mı kontrol et (aynı ID'ye sahip olmayan)
            var hasDuplicateProduct = await _repository.GetByQueryAsync(x =>
                x.ProductName.ToLower() == updateProductDto.ProductName.ToLower() &&
                x.ProductId != updateProductDto.ProductId);

            if (hasDuplicateProduct.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu isimde bir ürün zaten var, güncelleme yapılamaz."
                };
            }

            // Ürün bilgilerini güncelle
            existingProduct.ProductName = updateProductDto.ProductName;
            existingProduct.StockQuantity = updateProductDto.StockQuantity;
            existingProduct.Price = updateProductDto.Price;

            try
            {
                await _repository.UpdateAsync(existingProduct);
                await _unitOfWork.DbSaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Ürün güncelleme sırasında bir hata oluştu");
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Ürün başarıyla güncellendi"
            };
        }

        public async Task<ServiceMessage> DeleteProductAsync(int id)
        {
            // Silinecek ürün veritabanında var mı kontrol et
            var existingProduct = await _repository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Silmek istediğiniz ürün bulunamadı."
                };
            }

            try
            {
                // Ürünü sil
                await _repository.DeleteAsync(existingProduct);
                await _unitOfWork.DbSaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Ürün silme sırasında bir hata oluştu.");
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Ürün başarıyla silindi."
            };
        }

      
    }
}
