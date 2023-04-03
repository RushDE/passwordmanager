using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PasswordManagerServer
{
    public class Crypto
    {
        public static string CreateToken(IConfiguration configuration, string uuid)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, uuid)
            };
            var jwtTokenKey = configuration.GetSection("Jwt:TokenKey")?.Value;
            if (string.IsNullOrWhiteSpace(jwtTokenKey))
            {
                throw new Exception("Couldn't find the jwt token key.");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenKey));
            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha512Signature
            );
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(6),
                signingCredentials: credentials
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
