using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineShoppingPlatform.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == loginRequest.Email);
            if (user == null)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            // Şifreyi doğrulama (Data Protection kullanarak şifre doğrulama yapılabilir)
            if (user.Password != loginRequest.Password)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");

            if (secretKey == null)
            {
                // Handle the null value (e.g., throw an exception or set a default value)
                throw new ArgumentNullException("SecretKey", "Secret key cannot be null.");
            }

            var key = Encoding.UTF8.GetBytes(secretKey);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("ExpirationMinutes")),
                Issuer = jwtSettings.GetValue<string>("Issuer"),
                Audience = jwtSettings.GetValue<string>("Audience"),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
