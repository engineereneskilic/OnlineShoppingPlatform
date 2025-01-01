using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShoppingPlatform.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        [HttpGet]
        public IActionResult Home()
        {
            return Ok(new { Message = "Hoş geldin Admin.." });
        }
    }
}
