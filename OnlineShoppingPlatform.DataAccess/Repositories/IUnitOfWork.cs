using OnlineShoppingPlatform.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OnlineShoppingPlatform.DataAccess.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        //IRepository<User> Users { get; }
        //IRepository<Product> Products { get; }
        //IRepository<Order> Orders { get; }
        //IRepository<OrderProduct> OrderProducts { get; }

        // Generic repository yöntemi
        IRepository<T> Repository<T>() where T : class;

        Task<int> CommitAsync(); // Değişiklikleri kaydeder.
    }
}
