using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities.Logging
{
    public class RequestLog
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty; // İstek URL'si
        public DateTime RequestTime { get; set; } // İstek zamanı
        public string ClientId { get; set; } = string.Empty; // İsteği yapan müşterinin kimliği (isteğe bağlı)
    }
}
