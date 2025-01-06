using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Maintenance
{
    public class MaintenanceMode
    {
        [Key]
        [JsonIgnore] // Bu property'nin JSON çıktısında görünmemesini sağlar
        public int Id { get; set; } // Primary Key

        [Required]
        public bool IsActive { get; set; } // Bakım modunun aktiflik durumu

        public string? Message { get; set; } // İsteğe bağlı olarak özel mesaj gösterebilirsiniz


        [Required]
        public DateTime StartTime { get; set; } // Bakımın başlangıç zamanı

        public DateTime? EndTime { get; set; } // Bakımın bitiş zamanı (nullable)
    }
}
