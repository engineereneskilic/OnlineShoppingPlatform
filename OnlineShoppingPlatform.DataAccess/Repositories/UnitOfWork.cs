using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OnlineShoppingPlatform.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;

        // Repository örnekleri
        //public IRepository<User> Users { get; private set; }
        //public IRepository<Product> Products { get; private set; }
        //public IRepository<Order> Orders { get; private set; }
        //public IRepository<OrderProduct> OrderProducts { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            // Repository'lerin örneklenmesi
            //Users = new Repository<User>(_context);
            //Products = new Repository<Product>(_context);
            //Orders = new Repository<Order>(_context);
            //OrderProducts = new Repository<OrderProduct>(_context);
        }

        // Belirli bir entity için repository döner (Generic yöntem)
        public IRepository<T> Repository<T>() where T : class
        {
            return new Repository<T>(_context);
        }

        // Değişiklikleri veritabanına kaydetmek için
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Dispose metodu
        public void Dispose()
        {
            _context.Dispose();
        }


    }
}
