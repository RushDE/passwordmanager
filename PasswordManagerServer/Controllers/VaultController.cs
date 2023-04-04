using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordManagerServer.Data;
using PasswordManagerServer.Dtos;
using PasswordManagerServer.Models;

namespace PasswordManagerServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VaultController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VaultController(
            DataContext dataContext,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("CreatePasswordEntry")]
        public async Task<ActionResult<MessageDto>> CreatePasswordEntry(PasswordDto passwordDto)
        {
            if (passwordDto.Uuid != null)
            {
                return BadRequest(new MessageDto("The Uuid field has to be empty."));
            }
            string requestUuid = Auth.GetUuid(_httpContextAccessor);
            await _dataContext.PasswordEntries.AddAsync(
                new PasswordEntry()
                {
                    Uuid = Guid.NewGuid().ToString(),
                    UserUuid = requestUuid,
                    Name = passwordDto.EncryptedName,
                    Link = passwordDto.EncryptedLink,
                    Username = passwordDto.EncryptedUsername,
                    Password = passwordDto.EncryptedPassword
                }
            );
            await _dataContext.SaveChangesAsync();
            return Ok(new MessageDto("Added password."));
        }

        [HttpPatch("UpdatePasswordEntry")]
        public async Task<ActionResult<MessageDto>> UpdatePasswordEntry(PasswordDto passwordDto)
        {
            if (string.IsNullOrWhiteSpace(passwordDto.Uuid))
            {
                return BadRequest(new MessageDto("The Uuid field has to be given."));
            }
            string requestUuid = Auth.GetUuid(_httpContextAccessor);
            if (!_dataContext.PasswordEntries.Any(
                entry => entry.Uuid == passwordDto.Uuid && entry.UserUuid == requestUuid
                )
            )
            {
                return BadRequest(new MessageDto("Password entry doesen't exist."));
            }
            PasswordEntry entry = _dataContext.PasswordEntries.First(entry => entry.Uuid == passwordDto.Uuid);
            entry.Name = passwordDto.EncryptedName;
            entry.Link = passwordDto.EncryptedLink;
            entry.Username = passwordDto.EncryptedUsername;
            entry.Password = passwordDto.EncryptedPassword;
            await _dataContext.SaveChangesAsync();
            return Ok(new MessageDto("Updated the password."));
        }

        [HttpDelete("DeletePasswordEntry/{Uuid}")]
        public async Task<ActionResult<MessageDto>> DeletePasswordEntry(string Uuid)
        {
            string requestUuid = Auth.GetUuid(_httpContextAccessor);
            if (!_dataContext.PasswordEntries.Any(
                entry => entry.Uuid == Uuid && entry.UserUuid == requestUuid
                )
            )
            {
                return BadRequest(new MessageDto("Password entry doesen't exist."));
            }
            _dataContext.PasswordEntries
                .Remove(_dataContext.PasswordEntries
                    .First(entry => entry.Uuid == Uuid));
            await _dataContext.SaveChangesAsync();
            return Ok(new MessageDto("Deleted password."));
        }

        [HttpGet("ListPasswordEntrys")]
        public async Task<ActionResult<List<PasswordDto>>> ListPasswordEntrys()
        {
            string requestUuid = Auth.GetUuid(_httpContextAccessor);
            return (await _dataContext.PasswordEntries
                .Where(entry => entry.UserUuid == requestUuid)
                .ToListAsync())
                .Select(
                    entry => new PasswordDto()
                    {
                        Uuid = entry.Uuid,
                        EncryptedName = entry.Name,
                        EncryptedLink = entry.Link,
                        EncryptedUsername = entry.Username,
                        EncryptedPassword = entry.Password
                    }
                )
                .ToList();
        }
    }
}
