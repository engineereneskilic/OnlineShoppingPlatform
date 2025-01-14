using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShoppingPlatform.DataAccess.Entities;

namespace OnlineShoppingPlatform.DataAccess.Maintenance
{
    public class MaintenanceMode : BaseEntity
    {
        [Key]
        //[JsonIgnore] // Bu property'nin JSON çıktısında görünmemesini sağlar
        public int MaintenanceId { get; set; } // Primary Key

        [Required]
        public bool IsActive { get; set; } // Bakım modunun aktiflik durumu

        public string? Message { get; set; } // İsteğe bağlı olarak özel mesaj gösterebilirsiniz


        [Required]
        public DateTime StartTime { get; set; } // Bakımın başlangıç zamanı

        public DateTime? EndTime { get; set; } // Bakımın bitiş zamanı (nullable)
    }

    public class MaintenanceConfiguration : BaseConfigiration<MaintenanceMode>
    {
        public override void Configure(EntityTypeBuilder<MaintenanceMode> builder)
        {
            base.Configure(builder);

        }
    }
}
