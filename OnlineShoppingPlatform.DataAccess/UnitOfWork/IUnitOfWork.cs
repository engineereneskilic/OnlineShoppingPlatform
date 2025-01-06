using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OnlineShoppingPlatform.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        //IRepository<User> Users { get; }
        //IRepository<Product> Products { get; }
        //IRepository<Order> Orders { get; }
        //IRepository<OrderProduct> OrderProducts { get; }

        // Generic repository yöntemi
        IRepository<T> Repository<T>() where T : BaseEntity;

        Task BeginTransaction();
        // Task asenkron metodların voididir.

        Task CommitTransaction();

        Task RollBackTransaction();

        Task<int> DbSaveChangesAsync(); // Değişiklikleri kaydeder. Kaç işlem yapıldıysa int olarak o döner
    }
}
