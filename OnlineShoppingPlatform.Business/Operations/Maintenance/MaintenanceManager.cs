using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.Business.Operations.Maintenance.Dtos;
using OnlineShoppingPlatform.Business.Operations.Product.Dtos;
using OnlineShoppingPlatform.Business.Operations.User.Dtos;
using OnlineShoppingPlatform.Business.Types;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess.Maintenance;
using OnlineShoppingPlatform.DataAccess.Repositories;
using OnlineShoppingPlatform.DataAccess.UnitOfWork;

namespace OnlineShoppingPlatform.Business.Operations.Maintenance
{
    public class MaintenanceManager : IMaintenance
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<MaintenanceMode> _repository;

        public MaintenanceManager(IUnitOfWork unitOfWork, IRepository<MaintenanceMode> repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }


        public async Task<ServiceMessage<MaintenanceInfoDto>> ToggleLastMaintenanceModeOnOff()
        {
            // En son eklenen bakım kaydını tarih sırasına göre al
            var lastMaintenanceGetAll = await _repository.GetAllAsync();

            var lastMaintenance = lastMaintenanceGetAll
                                .OrderByDescending(x => x.StartTime)
                                .FirstOrDefault();

            if (lastMaintenance == null)
            {
                return new ServiceMessage<MaintenanceInfoDto>
                {
                    IsSucceed = false,
                    Message = "Herhangi bir bakım modu bulunmadığı için bakım modunu aktif yada pasif hale getiremezsiniz!"
                };
            }

            lastMaintenance.IsActive = lastMaintenance.IsActive ? false : true;

            try
            {
                await _repository.UpdateAsync(lastMaintenance);
                await _unitOfWork.DbSaveChangesAsync();

            }
            catch (Exception)
            {

                throw new Exception("Son bakım kaydı aktif edilmesi sırasında bir hata oluştu");
            }

            return new ServiceMessage<MaintenanceInfoDto>
            {
                IsSucceed = true,
                Message = "Son bakım kaydı aktiflik durumu başarıyla değiştirildi",
                Data = new MaintenanceInfoDto
                {
                    MaintenanceId = lastMaintenance.MaintenanceId,  // İlgili bakım kaydının Id'si
                    StartTime = lastMaintenance.StartTime,  // Bakımın başlama zamanı
                    EndTime = lastMaintenance.EndTime,  // Bakımın bitiş zamanı (nullable)
                    IsActive = lastMaintenance.IsActive,  // Bakımın aktiflik durumu
                    Message = lastMaintenance.Message  // Bakım mesajı
                }

            };

        }

        public async Task<ServiceMessage<MaintenanceInfoDto>> ToggleMaintenanceModeOnOff(int id)
        {
            // En son eklenen bakım kaydını tarih sırasına göre al
            var maintenance = await _repository.GetByIdAsync(id); 

            if (maintenance == null)
            {
                return new ServiceMessage<MaintenanceInfoDto>
                {
                    IsSucceed = false,
                    Message = $"{id} değerine sahip bakım modu bulunmadığı için bakım modunu aktif yada pasif hale getiremezsiniz!"
                };
            }

            maintenance.IsActive = maintenance.IsActive ? false : true;

            try
            {
                await _repository.UpdateAsync(maintenance);
                await _unitOfWork.DbSaveChangesAsync();

            }
            catch (Exception)
            {

                throw new Exception("Son bakım kaydı aktif edilmesi sırasında bir hata oluştu");
            }

            return new ServiceMessage<MaintenanceInfoDto>
            {
                IsSucceed = true,
                Message = "İlgili bakım kaydı aktiflik durumu başarıyla değiştirildi",
                Data = new MaintenanceInfoDto
                {
                    MaintenanceId = maintenance.MaintenanceId,  // İlgili bakım kaydının Id'si
                    StartTime = maintenance.StartTime,  // Bakımın başlama zamanı
                    EndTime = maintenance.EndTime,  // Bakımın bitiş zamanı (nullable)
                    IsActive = maintenance.IsActive,  // Bakımın aktiflik durumu
                    Message = maintenance.Message  // Bakım mesajı
                }

            };
        }


        public async Task<MaintenanceMode> GetMaintenanceByIdAsync(int id)
        {

            // id geçerli bir değer olup olmadığını kontrol et
            if (id <= 0)
            {
                throw new ArgumentException("Geçersiz Bakım kaydı ID'si", nameof(id));
            }

            // Veritabanında ürünü arayın
            var maintenance = await _repository.GetByIdAsync(id);

            // Eğer ürün bulunmazsa, null kontrolü yapın
            if (maintenance == null)
            {
                throw new KeyNotFoundException($"ID: {id} ile bir bakım kaydı bulunamadı.");
            }

            return maintenance;

            //return await _unitOfWork.Repository<ProductEntity>().GetByIdAsync(id);
        }

        public async Task<List<MaintenanceMode>> GetAllMaintenanceAsync()
        {
            // Tüm ürünleri veritabanından getir
            var maintenances = (await _repository.GetAllAsync());

            // Eğer hiçbir ürün bulunamazsa, boş bir liste döndür
            if (maintenances == null || !maintenances.Any())
            {
                throw new KeyNotFoundException("Veritabanında hiçbir maintenance bulunamadı.");
            }

            return maintenances.ToList();
        }

        public async Task<ServiceMessage> AddMaintenanceAsync(AddMaintenanceDto addMaintenanceDto)
        {


            var hasMaintenance = await _repository.GetByQueryAsync(
                x => !string.IsNullOrEmpty(x.Message) && !string.IsNullOrEmpty(addMaintenanceDto.Message) &&
                     x.Message.ToLower() == addMaintenanceDto.Message.ToLower()
            );

            if (hasMaintenance.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu bakım kaydı zaten bulunuyor, ekleyemezsiniz."
                };
            }

            var newMaintenanceMode = new MaintenanceMode
            {
                IsActive = addMaintenanceDto.IsActive,
                Message = addMaintenanceDto.Message,
                StartTime = addMaintenanceDto.StartTime,
                EndTime = addMaintenanceDto.EndTime
            };

            try
            {
                await _repository.AddAsync(newMaintenanceMode);
                await _unitOfWork.DbSaveChangesAsync();

            }
            catch (Exception)
            {

                throw new Exception("Bakım kaydı sırasında bir hata oluştu");
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Bakım Kaydı Başarıyla Eklendi"
            };
        }
        public async Task<ServiceMessage> UpdateMaintenanceAsync(UpdateMaintenanceDto updateMaintenanceDto)
        {
            // Veritabanından ilgili kaydı getir
            var existingMaintenance = await _repository.GetByIdAsync(updateMaintenanceDto.MaintenanceId);

            if (existingMaintenance == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Güncellemek istediğiniz bakım kaydı bulunamadı."
                };
            }

            //// Yeni mesaj veritabanında başka bir kayıtla eşleşiyor mu kontrol et
            //var query = await _repository.GetByQueryAsync(x =>
            //                x.MaintenanceId != updateMaintenanceDto.MaintenanceId &&
            //                (x.Message ?? string.Empty).Equals(updateMaintenanceDto.Message, StringComparison.OrdinalIgnoreCase)
            //            );

            var isMessageDuplicate = await _repository.GetByQueryAsync(
                x => x.MaintenanceId != updateMaintenanceDto.MaintenanceId &&
                     !string.IsNullOrEmpty(x.Message) && 
                    !string.IsNullOrEmpty(updateMaintenanceDto.Message) &&
                    x.Message.ToLower() == updateMaintenanceDto.Message.ToLower()
           );

            //var isMessageDuplicate = query.Any(); // IEnumerable üzerinden kontrol

            if (isMessageDuplicate.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Güncellemek istediğiniz bakım kaydı bulunamadı."
                };
            }

            // Güncelleme işlemi
            existingMaintenance.IsActive = updateMaintenanceDto.IsActive;
            existingMaintenance.StartTime = updateMaintenanceDto.StartTime;
            existingMaintenance.EndTime = updateMaintenanceDto.EndTime;
            existingMaintenance.Message = updateMaintenanceDto.Message;

            try
            {
                await _repository.UpdateAsync(existingMaintenance);
                await _unitOfWork.DbSaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Bakım güncelleme sırasında bir hata oluştu");
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Bakım başarıyla güncellendi"
            };
        }

        public async Task<ServiceMessage> DeleteMaintenanceAsync(int id)
        {
            // Silinecek ürün veritabanında var mı kontrol et
            var existingMaintenance = await _repository.GetByIdAsync(id);
            if (existingMaintenance == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Silmek istediğiniz bakım kaydı bulunamadı."
                };
            }

            try
            {
                // Ürünü sil
                await _repository.DeleteAsync(existingMaintenance);
                await _unitOfWork.DbSaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Bakım kaydı silme sırasında bir hata oluştu.");
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Bakım kaydı başarıyla silindi."
            };
        }

        public async Task<int> GetTotalCountMaintenances()
        {
            return await _repository.GetTotalCountsAsync();
        }
    }
}
