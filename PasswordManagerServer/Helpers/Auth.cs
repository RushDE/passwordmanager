using Microsoft.IdentityModel.Tokens;
using PasswordManagerServer.Data;
using PasswordManagerServer.Models;
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
        /// Creates a jwt bearer token from the user.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string CreateToken(IConfiguration configuration, User user)
        {
            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.Uuid),
                new Claim("tokenGeneration", user.TokenGeneration)
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
        /// Gets the request uuid from the httpContextAccessor, if the token is valid..
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        public static async Task<string?> ValidateAndGetUuid(IHttpContextAccessor httpContextAccessor, DataContext dataContext)
        {
            // Get the claims.
            ClaimsPrincipal claim = httpContextAccessor.HttpContext!.User;
            string uuid = claim.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            string tokenGeneration = claim.FindFirst("tokenGeneration")!.Value;

            // Get the user from the db.
            User? user = await dataContext.Users.FindAsync(uuid);
            if (user == null)
            {
                return null;
            }

            // Check if the token is valid.
            string validTokenGeneration = user.TokenGeneration;
            if (tokenGeneration != validTokenGeneration)
            {
                return null;
            }

            // Only return if the token is valid.
            return uuid;
        }
    }
}
