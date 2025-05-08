using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ServerApp.Helpers
{
    public class JwtTokenGenerator
    {
        public JwtTokenGenerator(IConfiguration c)
        {
            _secretKey = c.GetValue<string>("JwtSettings:SecretKey")!;
            _ttl = c.GetValue<int>("JwtSettings:TokenTtl");
        }
        private readonly string _secretKey;
        private readonly int _ttl;

        public string GenerateToken(string email, int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim("UserId", userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddSeconds(_ttl),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
