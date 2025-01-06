using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.DataAccess.Logging
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppDbContext _context;

        public LoggingMiddleware(RequestDelegate next, AppDbContext context)
        {
            _next = next;
            _context = context;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // İsteğin bilgilerini log tablosuna ekle
            var log = new RequestLog
            {
                Url = context.Request.Path,
                RequestTime = DateTime.UtcNow,
                ClientId = context.User?.Identity?.Name ?? "Anonymous" // Müşteri kimliği (varsa)
            };

            await _context.RequestLogs.AddAsync(log);
            await _context.SaveChangesAsync();

            // Bir sonraki middleware'e geç
            await _next(context);
        }
    }
}
