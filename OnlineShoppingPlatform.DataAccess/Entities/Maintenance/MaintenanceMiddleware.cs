using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Entities.Maintenance
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
            if (!requestPath.StartsWith("/api/maintenance"))
            {

                using (var scope = _serviceProvider.CreateScope()) // Scoped servisleri kullanmak için scope oluşturuyoruz
                {
                    var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // DbContext'i burada alıyoruz

                    var maintenanceMode = await _context.MaintenanceModes.FirstOrDefaultAsync();

                    if (maintenanceMode != null && maintenanceMode.IsActive)
                    {
                        var currentTime = DateTime.UtcNow;

                        // Bakım modunun geçerli olduğu zamanı kontrol et
                        if (currentTime >= maintenanceMode.StartTime &&
                            (!maintenanceMode.EndTime.HasValue || currentTime <= maintenanceMode.EndTime.Value))
                        {
                            //context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                            if (maintenanceMode.IsActive)
                            {
                                if (!string.IsNullOrEmpty(maintenanceMode.Message))
                                {
                                    await context.Response.WriteAsync($"Bakım Modu Aktif.\n  Bakım modu Mesajı: {maintenanceMode.Message}\n  Başlangıç Zamanı: {maintenanceMode.StartTime.ToString("g")}\n  Tahmini Bitiş Zamanı: {maintenanceMode.EndTime?.ToString("g") ?? "Belirtilmedi"}");
                                }
                                else
                                {
                                    await context.Response.WriteAsync($"Bakım Modu Aktif. Herhangi bir bilgi verilmedi.\n Tahmini Bitiş Zamanı:{maintenanceMode.EndTime?.ToString("g") ?? "Belirtilmedi"}");
                                }
                            }

                            return;
                        }
                    }
                }

                
            }
            await _next(context); // Bakım modunda değilse istekleri normal şekilde devam ettir
        }
    }
}
