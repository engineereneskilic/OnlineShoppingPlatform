using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess.Logging;
using OnlineShoppingPlatform.DataAccess.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
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


        // Model konfigürasyonu
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderProductConfiguration());

            modelBuilder.ApplyConfiguration(new RequestLogConfiguration());


            base.OnModelCreating(modelBuilder);

            // ÖZEL DÜZELTMELER

            // Order tablosu için TotalAmount hassasiyeti
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2); // 18 basamak, virgülden sonra 2 basamak

            // Product tablosu için Price hassasiyeti
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2); // 18 basamak, virgülden sonra 2 basamak

            // İLİŞKİLER

            // Many-to-Many ilişki konfigürasyonu
            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            // Order ile ilişkiyi belirt

            modelBuilder.Entity<OrderProduct>()
               .HasOne(op => op.Order) // Define the relationship to Order
               .WithMany(o => o.OrderProducts) // Define the reverse relationship
               .HasForeignKey(op => op.OrderId) // Specify the correct foreign key
               .OnDelete(DeleteBehavior.Cascade);

            // Product ile ilişkiyi belirt

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product) // Define the relationship to Product
                .WithMany(p => p.OrderProducts) // Define the reverse relationship
                .HasForeignKey(op => op.ProductId) // Specify the correct foreign key
                .OnDelete(DeleteBehavior.Cascade);


            // Benzersiz Email için Index
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Server=.;Database=OnlineShoppingPlatform;Trusted_Connection=True;MultipleActiveResultSets=true");
        //    }
        //}
    }
}
