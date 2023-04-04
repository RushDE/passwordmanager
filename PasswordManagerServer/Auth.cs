using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PasswordManagerServer
{
    /// <summary>
    /// Includes all the helper methods that do something with authentification.
    /// </summary>
    public class Auth
    {
        /// <summary>
        /// Creates a jwt bearer token from the user uuid.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
            int expirationInHours = configuration.GetValue<int>("Jwt:ExpirationInHours");
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(expirationInHours),
                signingCredentials: credentials
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return $"bearer {jwt}";
        }

        /// <summary>
        /// Gets the resquest uuid from the httpContextAccessor.
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        public static string GetUuid(IHttpContextAccessor httpContextAccessor)
        {
            var uuid = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            return uuid;
        }
    }
}
