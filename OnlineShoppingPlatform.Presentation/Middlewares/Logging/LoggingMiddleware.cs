using OnlineShoppingPlatform.DataAccess.Logging;
using OnlineShoppingPlatform.DataAccess;
using OnlineShoppingPlatform.Business.Operations.Maintenance;
using OnlineShoppingPlatform.DataAccess.Entities;

namespace OnlineShoppingPlatform.Presentation.Middlewares.Logging
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly AppDbContext _context;
        private readonly IServiceProvider _serviceProvider; // IServiceProvider kullanacağız


        public LoggingMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("içerde");
            try
            {
                var requestPath = context.Request.Path.ToString().ToLower();

                using (var scope = _serviceProvider.CreateScope()) // Scoped servisleri kullanmak için scope oluşturuyoruz
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // DbContext'i burada alıyoruz

                    //var userClaim = context.User.Claims.FirstOrDefault();

                    var user = context.User;
                    if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
                    { 
                        // User nesnesine HttpContext üzerinden erişim sağlıyoruz


                        // İsteğin bilgilerini log tablosuna ekle
                        var log = new RequestLog
                        {
                            Id = dbContext.RequestLogs.Count() + 1,
                            Url = requestPath,
                            RequestTime = DateTime.UtcNow,
                            ClientId = context.User?.Identity?.Name ?? "Anonymous", // Müşteri kimliği (varsa)
                            UserId = int.TryParse(user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value, out var id) && id > 0 ? id : 1
                        };

                        // Logu veritabanına kaydet
                        await dbContext.RequestLogs.AddAsync(log);
                        await dbContext.SaveChangesAsync();
                    }
                    //else { Console.WriteLine("Log: Kullanıcı bulunamadı."); }
                }
                
            }
            catch (Exception ex)
            {
                // Hata yönetimi
                // İsterseniz bir loglama mekanizması ekleyebilirsiniz
                Console.WriteLine($"Logging failed: {ex.Message}");
            }

            // Bir sonraki middleware'e geç
            await _next(context);
        }
    }
}
