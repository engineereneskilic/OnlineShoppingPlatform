using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IQueryable<T>> GetAllAsync();

        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        Task<IQueryable<T>> GetByQueryAsync(Expression<Func<T, bool>> filter);

        Task<T> GetByIdAsync(int id);

        Task<int> GetTotalCountsAsync();

        Task<bool> isFirstAsync();


        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteAsync(int id);

        
    }

}
