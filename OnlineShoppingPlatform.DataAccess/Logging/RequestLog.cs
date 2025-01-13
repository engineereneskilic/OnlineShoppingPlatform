using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShoppingPlatform.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Logging
{
    public class RequestLog : BaseEntity
    {
        public int Id { get; set; }

  
        public int UserId { get; set; } // Bu, User tablosundaki Id'yi referans alır
        public string Url { get; set; } = string.Empty; // İstek URL'si
        public DateTime RequestTime { get; set; } // İstek zamanı
        public string ClientId { get; set; } = string.Empty; // İsteği yapan müşterinin kimliği (isteğe bağlı)  = UserId

        // İlişki: RequestLog bir User'a ait olduğundan, User ile ilişkilendiriyoruz
        public User User { get; set; } = new User(); // İşlem kaydını yapan kullanıcı
    }

    public class RequestLogConfiguration : BaseConfigiration<RequestLog>
    {
        public override void Configure(EntityTypeBuilder<RequestLog> builder)
        {
            base.Configure(builder);
        }
    }
}
