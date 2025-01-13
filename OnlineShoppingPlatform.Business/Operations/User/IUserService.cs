using OnlineShoppingPlatform.Business.Operations.Product.Dtos;
using OnlineShoppingPlatform.Business.Operations.User.Dtos;
using OnlineShoppingPlatform.Business.Types;
using OnlineShoppingPlatform.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserEntity = OnlineShoppingPlatform.DataAccess.Entities.User;


namespace OnlineShoppingPlatform.Business.Operations.User
{
    public interface IUserService
    {

        Task<ServiceMessage<UserInfoDto>> LoginUserAsync(LoginUserDto loginUserDto);


        Task<UserEntity> GetUserByIdAsync(int id);
        Task<List<UserEntity>> GetAllUsersAsync();
        //Task<List<UserEntity>> GetByQueryAsync();

        Task<ServiceMessage> AddUserAsync(AddUserDto user); 
        Task<ServiceMessage> UpdateUserAsync(UpdateUserDto product);
        Task<ServiceMessage> DeleteUserAsync(int id);
    }
}
