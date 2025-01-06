using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OnlineShoppingPlatform.Business.Operations.User.Dtos;
using System.Runtime.CompilerServices;
using OnlineShoppingPlatform.Business.Operations.User;
using OnlineShoppingPlatform.Presentation.Models;
using OnlineShoppingPlatform.Presentation.Jwt;
using OnlineShoppingPlatform.Business.Types;
using Microsoft.AspNetCore.Authorization;

namespace OnlineShoppingPlatform.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        private readonly IUserService _userService;

        public AuthController(AppDbContext context, IConfiguration configuration, IUserService userService)
        {
            _context = context;
            _configuration = configuration;

            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _userService.LoginUser(
                    new LoginUserDto{ Email = loginRequest.Email, Password = loginRequest.Password }
                );

            if (!result.IsSucceed)
            {
                return Ok(new ServiceMessage()
                {
                    IsSucceed = result.IsSucceed,
                    Message = result.Message
                });
            }

            var user = result.Data;
            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User object cannot be null.");
            }

            if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
            {
                throw new ArgumentException("User's FirstName and LastName cannot be null or empty.");
            }

            var secretKey = configuration["JwtSettings:SecretKey"];
            var issuer = configuration["JwtSettings:Issuer"];
            var audience = configuration["JwtSettings:Audience"];
            var expireMinutesValue = configuration["JwtSettings:ExpirationMinutes"];

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(expireMinutesValue))
            {
                throw new ArgumentNullException("JwtSettings", "JWT settings cannot be null or empty in the configuration.");
            }

            if (!int.TryParse(expireMinutesValue, out var expireMinutes))
            {
                throw new FormatException("ExpireMinutes in JwtSettings must be a valid integer.");
            }

            var token = JwtHelper.GenerateJwtToken(new JwtDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                SecretKey = secretKey,
                Issuer = issuer,
                Audience = audience,
                ExpireMinutes = expireMinutes
            });

            // Bilgiler doğru ise token üretelim
            return Ok(new LoginResponse()
            {
                Message = "Giriş Başarıyla Gerçekleşti",
                Token = token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }  // TODO: İleride action filter olarak kodlanacak
            var addUserDto = new AddUserDto
            {
                Email = registerRequest.Email,
                UserName = registerRequest.UserName,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Password = registerRequest.Password,
                BirthDate = registerRequest.BirthDate
            };

            var result = await _userService.CreateUserAsync(addUserDto);

            if (result.IsSucceed)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message);
            }


        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMyUser()
        {

            return Ok("Hoşgeldiniz");
        } 

    }
}
