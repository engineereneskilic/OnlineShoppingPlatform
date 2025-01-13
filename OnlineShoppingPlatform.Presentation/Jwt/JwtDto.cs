using Microsoft.SqlServer.Server;
using OnlineShoppingPlatform.DataAccess.Entities.Enums;

namespace OnlineShoppingPlatform.Presentation.Jwt
{
    public class JwtDto
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public UserRole UserRole { get; set; }

        public string SecretKey { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public int ExpireMinutes { get; set; }
    }
}
