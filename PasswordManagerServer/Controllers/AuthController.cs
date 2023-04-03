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

        public AuthController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPut("Register")]
        public async Task<ActionResult<List<User>>> Register(UserDto userDto)
        {
            if (
                string.IsNullOrWhiteSpace(userDto.Username) ||
                string.IsNullOrWhiteSpace(userDto.PrehashedPassword)
                )
            {
                return BadRequest("Empty usernname or password.");
            }
            if (_dataContext.Users.Any(username => username.Username == userDto.Username))
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
    }
}
