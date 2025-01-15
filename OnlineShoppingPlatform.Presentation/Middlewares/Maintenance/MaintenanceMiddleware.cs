using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.Business.Operations.Maintenance;
using OnlineShoppingPlatform.DataAccess;

namespace OnlineShoppingPlatform.Presentation.Middlewares.Maintenance
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider; // IServiceProvider kullanacağız

        public MaintenanceMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestPath = context.Request.Path.ToString().ToLower();

            // Bakım modu kontrüllerinde bakım modu kontrolü yapmaya gerek yoktur
            if (!requestPath.StartsWith("/api/maintenance") && !requestPath.StartsWith("/api/auth/login"))
            {

                using (var scope = _serviceProvider.CreateScope()) // Scoped servisleri kullanmak için scope oluşturuyoruz
                {
                    var maintenanceService = scope.ServiceProvider.GetRequiredService<IMaintenance>(); // DbContext'i burada alıyoruz

                    var maintenanceMode = await maintenanceService.GetAllMaintenanceAsync();

                    if (maintenanceMode != null)
                    {

                        foreach (var mode in maintenanceMode)
                        {

                            if (mode != null && mode.IsActive)
                            {
                                var currentTime = DateTime.Now;

                                // Bakım modunun geçerli olduğu zamanı kontrol et
                                if (currentTime >= mode.StartTime &&
                                    (!mode.EndTime.HasValue || currentTime <= mode.EndTime.Value))
                                {
                                    //context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                                    if (mode.IsActive)
                                    {
                                        if (!string.IsNullOrEmpty(mode.Message))
                                        {
                                            context.Response.ContentType = "text/plain; charset=utf-8"; // Doğru encoding ayarı
                                            context.Response.Headers["Content-Type"] = "text/plain; charset=utf-8";
                                            await context.Response.WriteAsync($"Bakım Modu Aktif.\n  Bakım modu Mesajı: {mode.Message}\n  Başlangıç Zamanı: {mode.StartTime.ToString("g")}\n  Tahmini Bitiş Zamanı: {mode.EndTime?.ToString("g") ?? "Belirtilmedi"}");
                                            //await context.Response.WriteAsync("Bakım modu aktif");
                                        }
                                        else
                                        {
                                            context.Response.ContentType = "text/plain; charset=utf-8"; // Doğru encoding ayarı
                                            context.Response.Headers["Content-Type"] = "text/plain; charset=utf-8";
                                            await context.Response.WriteAsync($"Bakım Modu Aktif. Herhangi bir bilgi verilmedi.\n Tahmini Bitiş Zamanı:{mode.EndTime?.ToString("g") ?? "Belirtilmedi"}");
                                        }
                                    }

                                    return;
                                }
                            }
                        }
                    }
                    /**/

                }


            }
            await _next(context); // Bakım modunda değilse istekleri normal şekilde devam ettir
        }
    }
}
