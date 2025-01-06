using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            CreatedDate = DateTime.Now;
        }

        // public int Id { get; set; } // Primary Key
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Oluşturulma zamanı
        public DateTime? ModifiedDate { get; set; } // Güncellenme zamanı
        public bool IsDeleted { get; set; }
    }


    public abstract class BaseConfigiration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasQueryFilter(x => x.IsDeleted == false);
            // bu veritabanı üzerinde yapılacak bütün sorgulamalarda ve diğer linq işlemlerince geçerli olacak bir filtreleme yazdık. Böylelikle hiçbir zaman
            // bir daha soft delete atılmış verilerle uğraşmıicaz

            builder.Property(x => x.ModifiedDate).IsRequired(false);
        }
    }



}
