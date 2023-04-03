using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordManagerServer.Data;
using PasswordManagerServer.Dtos;
using PasswordManagerServer.Models;

namespace PasswordManagerServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(
            DataContext dataContext,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _dataContext = dataContext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<MessageDto>> Register(UserDto userDto)
        {
            if (
                string.IsNullOrWhiteSpace(userDto.Username) ||
                string.IsNullOrWhiteSpace(userDto.PrehashedPassword)
                )
            {
                return BadRequest(new MessageDto("Empty usernname or password."));
            }
            if (_dataContext.Users.Any(user => user.Username == userDto.Username))
            {
                return BadRequest(new MessageDto("Username already exists."));
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PrehashedPassword);
            await _dataContext.Users.AddAsync(new User
            {
                Uuid = Guid.NewGuid().ToString(),
                Username = userDto.Username,
                PasswordHash = passwordHash,
            });
            await _dataContext.SaveChangesAsync();
            return Ok(new MessageDto("Registered user."));
        }

        [HttpPost("Login")]
        public ActionResult<JwtBearerDto> Login(UserDto userDto)
        {
            if (!_dataContext.Users.Any(username => username.Username == userDto.Username))
            {
                return BadRequest(new JwtBearerDto("User doesen't exist."));
            }
            User user = _dataContext.Users.First(user => user.Username == userDto.Username);
            if (!BCrypt.Net.BCrypt.Verify(userDto.PrehashedPassword, user.PasswordHash))
            {
                return BadRequest(new JwtBearerDto("Invalid password."));
            }
            string token = Auth.CreateToken(_configuration, user.Uuid);
            return Ok(new JwtBearerDto("Successfull login.") { Token = token });
        }

        [HttpPatch("ChangePassword"), Authorize]
        public async Task<ActionResult<MessageDto>> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            string requestUuid = Auth.GetUuid(_httpContextAccessor);
            if (!_dataContext.Users.Any(user => user.Uuid == requestUuid))
            {
                return BadRequest(new MessageDto("User doesen't exist."));
            }
            User user = _dataContext.Users.First(user => user.Uuid == requestUuid);
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.PrehashedOldPassword, user.PasswordHash))
            {
                return BadRequest(new MessageDto("Invalid old password."));
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.PrehashedNewPassword);
            await _dataContext.SaveChangesAsync();
            return Ok(new MessageDto("Changed password."));
        }

        [HttpDelete("DeleteUser"), Authorize]
        public async Task<ActionResult<string>> DeleteUser(DeleteUserDto deleteUserDto)
        {
            string requestUuid = Auth.GetUuid(_httpContextAccessor);
            if (!_dataContext.Users.Any(user => user.Uuid == requestUuid))
            {
                return BadRequest(new MessageDto("User doesen't exist."));
            }
            User user = _dataContext.Users.First(user => user.Uuid == requestUuid);
            if (!BCrypt.Net.BCrypt.Verify(deleteUserDto.PrehashedPassword, user.PasswordHash))
            {
                return BadRequest(new MessageDto("Invalid password."));
            }

            await _dataContext.Users
                .Where(user => user.Uuid == requestUuid)
                .ForEachAsync(user => _dataContext.Users.Remove(user));
            await _dataContext.PasswordEntries
                .Where(entry => entry.UserUuid == requestUuid)
                .ForEachAsync(entry => _dataContext.PasswordEntries.Remove(entry));
            await _dataContext.SaveChangesAsync();
            return Ok(new MessageDto("Deleted user and all passwords."));
        }
    }
}
