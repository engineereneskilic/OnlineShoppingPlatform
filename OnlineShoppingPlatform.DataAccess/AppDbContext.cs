using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess.Logging;
using OnlineShoppingPlatform.DataAccess.Maintenance;
using OnlineShoppingPlatform.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess
{   // ApplicationUser
    //public class AppDbContext : IdentityDbContext<User>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        // DbSet tanımlamaları (Tablolar)
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }

        public DbSet<RequestLog> RequestLogs { get; set; } // her isteği loglamak için 
        public DbSet<MaintenanceMode> MaintenanceModes { get; set; } // Middleware için 

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Tüm decimal özellikler için varsayılan hassasiyet (Precision: 18, Scale: 2)
            configurationBuilder.Properties<decimal>()
                .HavePrecision(18, 2);

            base.ConfigureConventions(configurationBuilder);
        }


        // Model konfigürasyonu
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderProductConfiguration());

            modelBuilder.ApplyConfiguration(new RequestLogConfiguration());



            ConfigureSeedDatasAsync(modelBuilder);




            base.OnModelCreating(modelBuilder);

            // ÖZEL DÜZELTMELER
            ConfigurePrecisionSettings(modelBuilder);

            // İLİŞKİLER
            ConfigureRelationships(modelBuilder);

            // EK İNDİSLER
            ConfigureIndexes(modelBuilder);

        }

        private void ConfigureSeedDatasAsync(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MaintenanceMode>().HasData(
                   new MaintenanceMode
                   {
                       MaintenanceId = -1,
                       IsActive = true,
                       Message = "İlk kurulum bakımı (Varsayılan olarak)",
                       StartTime = DateTime.Now,
                       EndTime = DateTime.Now.AddHours(1)
                   }
                );
        }

        private void ConfigurePrecisionSettings(ModelBuilder modelBuilder)
        {
            // Order tablosu için TotalAmount hassasiyeti
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2); // 18 basamak, virgülden sonra 2 basamak

            // Product tablosu için Price hassasiyeti
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2); // 18 basamak, virgülden sonra 2 basamak

            // OrderProduct tablosu için UnitPrice hassasiyeti
            modelBuilder.Entity<OrderProduct>()
                .Property(op => op.UnitPrice)
                .HasPrecision(18, 2); // 18 basamak, virgülden sonra 2 basamak
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // -- OrderProduct --
            /**************************************************/
            // Many-to-Many ilişki konfigürasyonu
            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            // Order ile ilişki
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product ile ilişki
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // -- Order --
            /*************************************************/
            // User ile ilişkisi
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User) // Order, bir User'a ait
                .WithMany(u => u.Orders) // User, birçok Order'a sahip olabilir
                .HasForeignKey(o => o.CustomerId) // Order tablosunda UserId yabancı anahtarı
                .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silindiğinde, ona ait siparişler de silinir


            // -- Log --
            /*************************************************/
            // User ile ilişkisi
            modelBuilder.Entity<RequestLog>()
               .HasOne(al => al.User) // AuditLog tablosundaki her kayıt bir User'a ait olacak şekilde ilişkiyi tanımlıyoruz
               .WithMany(u => u.RequestLogs) // Bir User birden fazla AuditLog'a sahip olabilir, yani kullanıcı birden fazla işlem kaydına sahip olabilir
               .HasForeignKey(al => al.UserId) // AuditLog tablosundaki UserId alanı, User tablosundaki birincil anahtara (Id) yabancı anahtar olarak bağlanır
               .OnDelete(DeleteBehavior.Restrict); // Kullanıcı silindiğinde, o kullanıcıya ait işlem loglarının silinmesini engelliyoruz. Yani kullanıcı silinse de loglar korunur.


        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            // User tablosunda benzersiz Email için Index
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

    }
}
