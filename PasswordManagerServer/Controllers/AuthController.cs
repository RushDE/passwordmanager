using Microsoft.AspNetCore.Mvc;
using PasswordManagerServer.Data;
using PasswordManagerServer.Dtos;
using PasswordManagerServer.Models;

namespace PasswordManagerServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        public AuthController(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(UserDto userDto)
        {
            if (
                string.IsNullOrWhiteSpace(userDto.Username) ||
                string.IsNullOrWhiteSpace(userDto.PrehashedPassword)
                )
            {
                return BadRequest("Empty usernname or password.");
            }
            if (_dataContext.Users.Any(user => user.Username == userDto.Username))
            {
                return BadRequest("Username already exists.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PrehashedPassword);
            await _dataContext.Users.AddAsync(new User
            {
                Uuid = Guid.NewGuid().ToString(),
                Username = userDto.Username,
                PasswordHash = passwordHash,
            });
            await _dataContext.SaveChangesAsync();
            return Ok("Registered user.");
        }

        [HttpPost("Login")]
        public ActionResult Login(UserDto userDto)
        {
            if (!_dataContext.Users.Any(username => username.Username == userDto.Username))
            {
                return BadRequest("User doesen't exist.");
            }
            User user = _dataContext.Users.First(user => user.Username == userDto.Username);
            if (!BCrypt.Net.BCrypt.Verify(userDto.PrehashedPassword, user.PasswordHash))
                {
                return BadRequest("Invalid password.");
            }
            string token = Crypto.CreateToken(_configuration, user.Uuid);
            return Ok(token);
        }

        [HttpPatch("ChangePassword")]
        public async Task<ActionResult> ChangePassword()
        {
            throw new NotImplementedException();
        }
    }
}
