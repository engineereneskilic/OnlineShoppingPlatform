using OnlineShoppingPlatform.DataAccess.UnitOfWork;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess.Entities.Enums;
using OnlineShoppingPlatform.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShoppingPlatform.Business.Types;
using OnlineShoppingPlatform.Business.Operations.User.Dtos;
using OnlineShoppingPlatform.DataAccess.Repositories;
using OnlineShoppingPlatform.Business.DataProtection;
using Microsoft.EntityFrameworkCore;
using UserEntity = OnlineShoppingPlatform.DataAccess.Entities.User;
using OnlineShoppingPlatform.Business.Operations.Product.Dtos;


namespace OnlineShoppingPlatform.Business.Operations.User
{
    public class UserManager : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IRepository<UserEntity> _userepository;

        private readonly IDataProtection _dataProtector;

        public UserManager(IUnitOfWork unitOfWork, IRepository<UserEntity> userrepository, IDataProtection dataProtector)
        {
            _unitOfWork = unitOfWork;
            _userepository = userrepository;
            _dataProtector = dataProtector;
        }

   
        public async Task<ServiceMessage> AddUserAsync(AddUserDto user)
        {


            if (user == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Kullanıcı bilgileri boş olamaz!"
                };
            }

            if (string.IsNullOrEmpty(user.Email) || !user.Email.Contains("@"))
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Geçerli bir email adresi giriniz!"
                };
            }

            // E-posta kontrolü
            var existingUser = await _userepository.GetByQueryAsync(x => x.Email.ToLower() == user.Email.ToLower());
            if (existingUser.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu e-posta adresi zaten kullanılmaktadır."
                };
            }
            // Eğer kayıtlıysa sisteme ilk gelen kullanıcı mı olup olmadığı bulunuyor. Eğer öyleyse rolü Admin olarak atanıyor eğer deilse sonradan gelenler Customer olacak şekilde atanıyor.

            bool isFirstUser = await _userepository.isFirstAsync();


                var newuser = new UserEntity()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Password = _dataProtector.Protect(user.Password), // Şifreleme olcak
                    BirthDate = user.BirthDate,
                    UserType = isFirstUser ? UserRole.Admin : UserRole.Customer,
                };

                await _userepository.AddAsync(newuser);


                try
                {
                    await _unitOfWork.DbSaveChangesAsync();

                }
                catch (Exception)
                {

                    throw new Exception("Kullanıcı kaydı sırasında bir hata oluştu");
                }

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "Kullanıcı başarıyla eklendi"
                };

            
           
        }

      


        public async Task<ServiceMessage<UserInfoDto>> LoginUserAsync(LoginUserDto loginUserDto)
        {

            if (loginUserDto == null || string.IsNullOrEmpty(loginUserDto.Email) || string.IsNullOrEmpty(loginUserDto.Password))
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = "E-posta ve şifre alanları boş olamaz!"
                };
            }

            var user = await _userepository.GetAsync(x => x.Email.ToLower() == loginUserDto.Email.ToLower());

            if(user is null )
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = "Kullanıcı bulunamadı"
                };
            }

            var unprotectedPassword = _dataProtector.UnProtect(user.Password);
            if(unprotectedPassword == loginUserDto.Password)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = true,
                    Data = new UserInfoDto
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserType = user.UserType
                    }
                };
            } else
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    //Message = unprotectedPassword + " -- "+ user.Password
                    Message = "Kullanıcı adı veya şifre yanlış"
                };
            }
        }

        public async Task<List<UserEntity>> GetAllUsersAsync()
        {
            var users = await _userepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                throw new Exception("Kayıtlı kullanıcı bulunamadı.");
            }

            return users.ToList();
        }

        public async Task<UserEntity> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Geçersiz kullanıcı ID'si.");
            }

            var user = await _userepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            return user;
        }


        public async Task<ServiceMessage> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            if (updateUserDto == null || updateUserDto.UserId <= 0)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Geçersiz kullanıcı bilgileri."
                };
            }

            var user = await _userepository.GetByIdAsync(updateUserDto.UserId);
            if (user == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Güncellenecek kullanıcı bulunamadı."
                };
            }

            // Kullanıcı bilgilerini güncelle
            user.FirstName = updateUserDto.FirstName ?? user.FirstName;
            user.LastName = updateUserDto.LastName ?? user.LastName;
            user.Email = updateUserDto.Email ?? user.Email;

            try
            {
                await _userepository.UpdateAsync(user);
                await _unitOfWork.DbSaveChangesAsync();

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "Kullanıcı başarıyla güncellendi."
                };
            }
            catch (Exception ex)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"Güncelleme sırasında bir hata oluştu: {ex.Message}"
                };
            }
        }

        public async Task<ServiceMessage> DeleteUserAsync(int id)
        {
            if (id <= 0)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Geçersiz kullanıcı ID'si."
                };
            }

            var user = await _userepository.GetByIdAsync(id);
            if (user == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Silinecek kullanıcı bulunamadı."
                };
            }

            try
            {
                await _userepository.DeleteAsync(user);
                await _unitOfWork.DbSaveChangesAsync();

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "Kullanıcı başarıyla silindi."
                };
            }
            catch (Exception ex)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"Kullanıcı silinirken bir hata oluştu: {ex.Message}"
                };
            }
        }

     

        /***********************************************************************/



    }
}
