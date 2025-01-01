using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace OnlineShoppingPlatform.Filters
{
    public class TimeRestrictedAccessFilter : ActionFilterAttribute
    {
        private readonly TimeSpan _startTime;
        private readonly TimeSpan _endTime;

        public TimeRestrictedAccessFilter(string startTime, string endTime)
        {
            // Zaman aralığını TimeSpan'e dönüştür
            _startTime = TimeSpan.Parse(startTime);
            _endTime = TimeSpan.Parse(endTime);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var currentTime = DateTime.Now.TimeOfDay;

            if (currentTime < _startTime || currentTime > _endTime)
            {
                context.Result = new ContentResult
                {
                    Content = $"API requests are allowed only between {_startTime} and {_endTime}.",
                    ContentType = "text/plain",
                    StatusCode = 403 // Forbidden
                };
            }
        }
    }
}
