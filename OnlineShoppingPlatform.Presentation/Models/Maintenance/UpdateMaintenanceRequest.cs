using OnlineShoppingPlatform.Business.Validations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineShoppingPlatform.Presentation.Models.Maintenance
{
    public class UpdateMaintenanceRequest
    {

        [JsonIgnore]
        public int MaintenanceId { get; set; } // Primary Key

        /// Bakım modunun aktiflik durumu (zorunlu).
        [Required(ErrorMessage = "IsActive alanı zorunludur.")]
        public bool IsActive { get; set; }


        /// Bakımın başlangıç zamanı (zorunlu).
        [Required(ErrorMessage = "Başlangıç zamanı zorunludur.")]
        [DataType(DataType.DateTime, ErrorMessage = "Geçerli bir tarih ve saat giriniz.")]
        public DateTime StartTime { get; set; }


        /// Bakımın bitiş zamanı (isteğe bağlı).
        [DataType(DataType.DateTime, ErrorMessage = "Geçerli bir tarih ve saat giriniz.")]
        [CompareDates(nameof(StartTime), ErrorMessage = "Bitiş zamanı başlangıç zamanından önce olamaz.")]
        public DateTime? EndTime { get; set; }


        /// Kullanıcılara gösterilecek özel mesaj (isteğe bağlı).
        [StringLength(500, ErrorMessage = "Mesaj 500 karakterden uzun olamaz.")]
        public string? Message { get; set; }
    }
}
