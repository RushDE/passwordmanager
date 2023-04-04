using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordManagerServer.Data;
using PasswordManagerServer.Dtos;
using PasswordManagerServer.Models;

namespace PasswordManagerServer.Controllers
{
    /// <summary>
    /// Endpoint to handle everything with the user account.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Injects everything.
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="configuration"></param>
        /// <param name="httpContextAccessor"></param>
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

        /// <summary>
        /// Creates a new account for a user.
        /// </summary>
        /// <param userDto="The name has to be unique."></param>
        /// <returns MessageDto="Explaines the success or error."></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {
        ///         "username": "Max",
        ///         "prehashedPassword": "hunter2" // Should be already hashed once with bcrypt.
        ///     }
        ///     
        /// Sample response:
        /// 
        ///     {
        ///         "message": "Registered user."
        ///     }
        /// </remarks>
        /// <response code="200">Created the user, and returns a success message.</response>
        /// <response code="400">Returns a error message.</response>
        [HttpPost("Register"), AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Returns a bearer token, if the credentials are valid.
        /// </summary>
        /// <param userDto="The account has to be already registered."></param>
        /// <returns JwtBearerDto="The token if the login is successfull, and a message that explaines the succes or error."></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {
        ///         "username": "Max",
        ///         "prehashedPassword": "hunter2" // Should be already hashed once with bcrypt.
        ///     }
        ///     
        /// Sample response:
        /// 
        ///     {
        ///         "message": "Successfull login.",
        ///         "token": "bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImM1M2JmNDRmLTY0ZTItNGQ3MS1hZWE5LTQzZDFlOWYxNDcxNSIsImV4cCI6MTY4MDYyMjM1MX0.AXe5fa_AgboS3740xAhJ5imY__7VLJKjQkJ5oxwDyrkSt904EmbBvdBudzcyeqbEMvHH7tgmB8gwnks47h6ztA"
        ///     }
        /// </remarks>
        /// <response code="200">Returns the token and a success message.</response>
        /// <response code="400">Returns a error message.</response>
        [HttpPost("Login"), AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(JwtBearerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JwtBearerDto), StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Changes the password of a user.
        /// </summary>
        /// <param changePasswordDto="The old and new prehashed password.."></param>
        /// <returns MessageDto="Explaines the success or error."></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {
        ///         "prehashedOldPassword": "hunter2",  // Should be already hashed once with bcrypt.
        ///         "prehashedNewPassword": "password"  // Should be already hashed once with bcrypt.
        ///     }
        ///     
        /// Sample response:
        /// 
        ///     {
        ///         "message": "Changed password."
        ///     }
        /// </remarks>
        /// <response code="200">Returns a success message.</response>
        /// <response code="400">Returns a error message.</response>
        [HttpPatch("ChangePassword")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status400BadRequest)]
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


        /// <summary>
        /// Deletes a user and all their passwords.
        /// </summary>
        /// <param deleteUserDto="Contains the prehashed password, for extry security."></param>
        /// <returns MessageDto="Explaines the success or error."></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {
        ///         "prehashedPassword": "hunter2" // Should be already hashed once with bcrypt.
        ///     }
        ///     
        /// Sample response:
        /// 
        ///     {
        ///         "message": "Deleted user and all passwords."
        ///     }
        /// </remarks>
        /// <response code="200">Returns a success message.</response>
        /// <response code="400">Returns a error message.</response>
        [HttpDelete("DeleteUser")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MessageDto>> DeleteUser(DeleteUserDto deleteUserDto)
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
