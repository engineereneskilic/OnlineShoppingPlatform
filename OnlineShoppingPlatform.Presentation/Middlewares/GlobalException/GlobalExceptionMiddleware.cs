using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingPlatform.Presentation.Middlewares.GlobalException
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Bir sonraki middleware'e devam et
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Global hata yakalandı.");
                await HandleExceptionAsync(context, ex); // Hata yönetimini burada ele al
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                DbUpdateException => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError // Diğer hatalar için varsayılan durum kodu
            };

            var errorResponse = new
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message,
                Details = exception.InnerException?.Message
            };

            return context.Response.WriteAsJsonAsync(errorResponse);
        }
    }

}
