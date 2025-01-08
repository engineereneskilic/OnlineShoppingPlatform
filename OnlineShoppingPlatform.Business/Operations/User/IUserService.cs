using OnlineShoppingPlatform.Business.Operations.User.Dtos;
using OnlineShoppingPlatform.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.Business.Operations.User
{
    public interface IUserService
    {
        Task<ServiceMessage> CreateUserAsync(AddUserDto user); // async çünkü unit of work kullanılacak

        Task<ServiceMessage<UserInfoDto>> LoginUserAsync(LoginUserDto loginUserDto);



        //Task<List<User>> GetAllUsersAsync();
        //Task<User> GetUserByIdAsync(int id);

        //Task<User> UpdateUserAsync(User user);

        //Task DeleteUserAsync(int id);
    }
}
