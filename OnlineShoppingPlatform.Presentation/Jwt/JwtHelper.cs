using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OnlineShoppingPlatform.DataAccess.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineShoppingPlatform.Presentation.Jwt
{
    public static class JwtHelper
    {
        public static string GenerateJwtToken(JwtDto jwtInfo)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtInfo.SecretKey));

            //credentials > kimlik bilgileri
            var credentials = new SigningCredentials(secretKey,SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtClaimNames.Id, jwtInfo.Id.ToString()),
                new Claim(JwtClaimNames.FirstName, jwtInfo.FirstName.ToString()),
                new Claim(JwtClaimNames.LastName, jwtInfo.LastName.ToString()),
                new Claim(JwtClaimNames.Email, jwtInfo.Email.ToString()),

                new Claim(JwtClaimNames.UserType, jwtInfo.UserRole.ToString()),
                new Claim(ClaimTypes.Role, jwtInfo.UserRole.ToString())
            };

            var expiredTime = DateTime.Now.AddMinutes(jwtInfo.ExpireMinutes); // şuanın saatine ilgili verilen süre kadar(45 dakika) ekle.

            var tokenDescriptor = new JwtSecurityToken(jwtInfo.Issuer, jwtInfo.Audience, claims, null, expiredTime, credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return token;
        }
    }
}
