using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PasswordManagerServer.Helpers
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
            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, uuid)
            ];
            string? jwtTokenKey = configuration.GetSection("Jwt:TokenKey")?.Value;
            if (string.IsNullOrWhiteSpace(jwtTokenKey))
            {
                throw new Exception("Couldn't find the jwt token key.");
            }
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(jwtTokenKey));
            SigningCredentials credentials = new(
                key,
                SecurityAlgorithms.HmacSha512Signature
            );
            int expirationInHours = configuration.GetValue<int>("Jwt:ExpirationInHours");
            JwtSecurityToken token = new(
                claims: claims,
                expires: DateTime.Now.AddHours(expirationInHours),
                signingCredentials: credentials
            );
            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return $"bearer {jwt}";
        }

        /// <summary>
        /// Gets the resquest uuid from the httpContextAccessor.
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        public static string GetUuid(IHttpContextAccessor httpContextAccessor)
        {
            string uuid = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            return uuid;
        }
    }
}
