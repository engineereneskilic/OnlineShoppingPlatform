using OnlineShoppingPlatform.DataAccess.UnitOfWork;
using OnlineShoppingPlatform.DataAccess.Entities;
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

                var newuser = new DataAccess.Entities.User()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Password = _dataProtector.Protect(user.Password), // Şifreleme olcak
                    BirthDate = user.BirthDate,
                    UserType = user.UserType
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

        public ServiceMessage<UserInfoDto> LoginUser(LoginUserDto loginUserDto)
        {
             var user = _userepository.Get(x => x.Email.ToLower() == loginUserDto.Email.ToLower());

            if(user is null )
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = "sdfsdfsddsfsdsdfsdf"
                };
            }

            var unprotectedPassword = _dataProtector.UnProtect(user.Result.Password);
            if(unprotectedPassword == loginUserDto.Password)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = true,
                    Data = new UserInfoDto
                    {
                        Email = user.Result.Email,
                        FirstName = user.Result.FirstName,
                        LastName = user.Result.LastName,
                        UserType = user.Result.UserType
                    }
                };
            } else
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = unprotectedPassword + " -- "+ user.Result.Password
                };
            }
        }

        /***********************************************************************/



    }
}
