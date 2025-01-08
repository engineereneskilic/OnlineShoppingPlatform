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

namespace OnlineShoppingPlatform.Business.Operations.User
{
    public class UserManager : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IRepository<DataAccess.Entities.User> _userepository;

        private readonly IDataProtection _dataProtector;

        public UserManager(IUnitOfWork unitOfWork, IRepository<DataAccess.Entities.User> userrepository, IDataProtection dataProtector)
        {
            _unitOfWork = unitOfWork;
            _userepository = userrepository;
            _dataProtector = dataProtector;
        }

        public async Task<ServiceMessage> CreateUserAsync(AddUserDto user)
        {
             // gelen kullanıcı email adresi ile daha önceden kayıt olup olmadığı kontrol ediliyor
            if (user.Email != null)
            {
                // Email kontrolü
                var hasMail = await _userepository.GetByQueryAsync(x => x.Email.ToLower() == user.Email.ToLower());

                if (hasMail.Any())
                {
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Email adresi zaten mevcut"
                    };
                }
                // Eğer kayıtlıysa sisteme ilk gelen kullanıcı mı olup olmadığı bulunuyor. Eğer öyleyse rolü Admin olarak atanıyor eğer deilse sonradan gelenler Customer olacak şekilde atanıyor.

                bool isFirstUser = await _userepository.isFirstAsync();


                var newuser = new DataAccess.Entities.User()
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
               
            } else
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Lütfen geçerli bir email adresi giriniz !"
                };
            }
        }



        public async Task<ServiceMessage<UserInfoDto>> LoginUserAsync(LoginUserDto loginUserDto)
        {
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

        /***********************************************************************/



    }
}
