using OnlineShoppingPlatform.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _unitOfWork.Repository<User>().GetByIdAsync(id);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return (await _unitOfWork.Repository<User>().GetAllAsync()).ToList();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.CommitAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            await _unitOfWork.Repository<User>().UpdateAsync(user);
            await _unitOfWork.CommitAsync();
            return user;
        }

        public async Task DeleteUserAsync(int id)
        {
            await _unitOfWork.Repository<User>().DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }
    }
}
