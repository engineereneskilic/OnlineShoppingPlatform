using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IQueryable<T>> GetAllAsync()
        {
            return await Task.FromResult(_dbSet.AsQueryable());
        }


        public async Task<IQueryable<T>> GetByQueryAsync(Expression<Func<T, bool>> filter)
        {
            return await Task.FromResult(_dbSet.Where(filter));  // IQueryable'ı döndürür, ancak asenkron çalıştırır.
        }

        public async Task<T> GetByIdAsync(int id)
        {

            // Try to find the entity by its ID
            var entity = await _dbSet.FindAsync(id);

            // If no entity is found, throw an exception or handle the null case
            if (entity == null)
            {
                //throw new InvalidOperationException($"Entity with ID {id} not found.");
                return null;
            }

            return entity;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            // Asenkron olarak veritabanından entity'yi bul
            var entity = await _dbSet.FirstOrDefaultAsync(predicate);

            // Eğer entity bulunamazsa, özel bir durum fırlat
            if (entity == null)
            {
                // throw new InvalidOperationException($"Entity not found.");
                return null;
            }

            return entity;
        }

        public async Task AddAsync(T entity)
        {
            
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }



        public async Task UpdateAsync(T entity)
        {
            entity.ModifiedDate = DateTime.Now;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }



        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }

        public async Task DeleteAsync(T entity)
        {
            //entity.ModifiedDate = DateTime.Now;
            //entity.IsDeleted = true;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> isFirstAsync()
        {
            // Veritabanında hiç kayıt olup olmadığını kontrol et
            var count = await _dbSet.CountAsync(); // Asenkron olarak saymayı bekle

            return count == 0; // Eğer sayılar sıfırsa, bu ilk kayıt demektir
        }

        public async Task<int> GetTotalCountsAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}
