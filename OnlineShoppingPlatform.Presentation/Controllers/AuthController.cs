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
using OnlineShoppingPlatform.Presentation.Jwt;
using OnlineShoppingPlatform.Business.Types;
using Microsoft.AspNetCore.Authorization;
using OnlineShoppingPlatform.Presentation.Models.Auth;

namespace OnlineShoppingPlatform.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            // Login işlemi
            var result = await _userService.LoginUserAsync(
                new LoginUserDto { Email = loginRequest.Email, Password = loginRequest.Password }
            );

            // Giriş başarısızsa
            if (!result.IsSucceed)
            {
                return Ok(new ServiceMessage
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
                Id = user.UserId,
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

            // Senaryo : Eğer daha önceden veritabanına kayıtlı biri yoksa rolü admin ata ondan sonra gelenler hep Customer olsun
            //var resultFirstOne = await _use

            
            var addUserDto = new AddUserDto
            {
              
                UserName = registerRequest.UserName,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Email = registerRequest.Email,
                Password = registerRequest.Password,
                PhoneNumber = registerRequest.PhoneNumber,
                BirthDate = registerRequest.BirthDate
            };


            

            var result = await _userService.AddUserAsync(addUserDto);

            if (result.IsSucceed)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }


        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyUserAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            if(userId == null) 
            {
                return Unauthorized("Kullanıcı bilgileri bulunamadı.");

            }

            if (int.Parse(userId) == 0) userId = "1";

            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            foreach (var claim in claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }

      
            var user = await _userService.GetUserByIdAsync(int.Parse(userId!));

            if (user == null)
            {
                return Unauthorized("Kullanıcı bulunamadı.");
            }
          
            return Ok(new UserInfoDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                UserType = user.UserType,
            });
        }

    }
}
