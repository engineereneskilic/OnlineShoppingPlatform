using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShoppingPlatform.Business.Validations;

namespace OnlineShoppingPlatform.Business.Operations.Maintenance.Dtos
{
    public class MaintenanceInfoDto
    {

        public int MaintenanceId;


        /// Bakım modunun aktiflik durumu (zorunlu).
        public bool IsActive { get; set; }


        /// Bakımın başlangıç zamanı (zorunlu).
        public DateTime StartTime { get; set; }


        /// Bakımın bitiş zamanı (isteğe bağlı).
        public DateTime? EndTime { get; set; }

        public string? Message { get; set; }
    }
}
