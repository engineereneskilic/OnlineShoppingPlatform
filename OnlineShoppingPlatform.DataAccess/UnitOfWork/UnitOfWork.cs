using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OnlineShoppingPlatform.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;


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
        public IRepository<T> Repository<T>() where T : BaseEntity
        {
            return new Repository<T>(_context);
        }

        // Değişiklikleri veritabanına kaydetmek için
        public async Task<int> DbSaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Dispose metodu
        public void Dispose()
        {
            _context.Dispose();
            // Garbage collector'a sen bunu temizleyebilirsin izni verdiğimiz yer
            // o an silmiyor, silinebilir yapıyor
            // GC.Collect()
            // GC.WaitForPendingFinalizers();
            // Bu kodlar Garbage collector'ı direk çalıştırır
        }


        // TRANSACTIONS
        public async Task BeginTransaction()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            await _context.Database.CommitTransactionAsync();   
        }

        public async Task RollBackTransaction()
        {
            await _transaction.RollbackAsync();
        }
    }
}
