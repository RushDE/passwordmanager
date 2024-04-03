using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerServer.Data;
using PasswordManagerServer.Dtos;
using PasswordManagerServer.Helpers;
using PasswordManagerServer.Models;

namespace PasswordManagerServer.Controllers
{
    /// <summary>
    /// Endpoint to handle everything with the passwords.
    /// </summary>
    /// <remarks>
    /// Injects everything.
    /// </remarks>
    /// <param name="dataContext"></param>
    /// <param name="httpContextAccessor"></param>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VaultController(
        DataContext dataContext,
        IHttpContextAccessor httpContextAccessor
        ) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// Creates a new password for a user, all information, ahould be encrypted client side.
        /// </summary>
        /// <param passwordDto="All the information for the new password."></param>
        /// <returns MessageDto="Explaines the success or error."></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {
        ///         "encryptedName": "Netflix", // Should be encrypted client side.
        ///         "encryptedLink": "https://netflix.com", // Should be encrypted client side.
        ///         "encryptedUsername": "Max", // Should be encrypted client side.
        ///         "encryptedPassword": "hunter2" // Should be encrypted client side.
        ///     }
        ///     
        /// Sample response:
        /// 
        ///     {
        ///         "message": "Added password."
        ///     }
        /// </remarks>
        /// <response code="200">Created the password, and returns a success message.</response>
        /// <response code="400">Returns a error message.</response>
        [HttpPost("CreatePasswordEntry")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MessageDto>> CreatePasswordEntry(PasswordDto passwordDto)
        {
            if (passwordDto.Uuid != null)
            {
                return BadRequest(new MessageDto("The Uuid field has to be empty."));
            }
            string? requestUuid = await Auth.ValidateAndGetUuid(_httpContextAccessor, _dataContext);
            if (requestUuid == null)
            {
                return BadRequest(new MessageDto("Invalid token."));
            }
            _ = await _dataContext.PasswordEntries.AddAsync(
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
            _ = await _dataContext.SaveChangesAsync();
            return Ok(new MessageDto("Added password."));
        }

        /// <summary>
        /// Changes the information about a password, all information, ahould be encrypted client side.
        /// </summary>
        /// <param passwordDto="All the information for the updated password."></param>
        /// <returns MessageDto="Explaines the success or error."></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {
        ///         "encryptedName": "Minecraft", // Should be encrypted client side.
        ///         "encryptedLink": "https://minecraft.com", // Should be encrypted client side.
        ///         "encryptedUsername": "xX_MineBoy_420_69_Xx", // Should be encrypted client side.
        ///         "encryptedPassword": "hunter2" // Should be encrypted client side.
        ///     }
        ///     
        /// Sample response:
        /// 
        ///     {
        ///         "message": "Updated the password."
        ///     }
        /// </remarks>
        /// <response code="200">Updated the password, and returns a success message.</response>
        /// <response code="400">Returns a error message.</response>
        [HttpPatch("UpdatePasswordEntry")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MessageDto>> UpdatePasswordEntry(PasswordDto passwordDto)
        {
            string? requestUuid = await Auth.ValidateAndGetUuid(_httpContextAccessor, _dataContext);
            if (requestUuid == null)
            {
                return BadRequest(new MessageDto("Invalid token."));
            }
            try
            {
                Vault.UpdatePasswordEntry(_dataContext, requestUuid, passwordDto);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new MessageDto(exception.Message));
            }
            _ = await _dataContext.SaveChangesAsync();
            return Ok(new MessageDto("Updated the password."));
        }


        /// <summary>
        /// Deletes a passwort by uuid.
        /// </summary>
        /// <param Uuid="The unique idenntifier of the password."></param>
        /// <returns MessageDto="Explaines the success or error."></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/DeletePasswordEntry/3abbead3-4c20-4baf-94ae-03a45e239521
        ///     
        /// Sample response:
        /// 
        ///     {
        ///         "message": "Deleted the password."
        ///     }
        /// </remarks>
        /// <response code="200">Deleted the password, and returns a success message.</response>
        /// <response code="400">Returns a error message.</response>
        [HttpDelete("DeletePasswordEntry/{Uuid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MessageDto>> DeletePasswordEntry(string Uuid)
        {
            string? requestUuid = await Auth.ValidateAndGetUuid(_httpContextAccessor, _dataContext);
            if (requestUuid == null)
            {
                return BadRequest(new MessageDto("Invalid token."));
            }
            if (!_dataContext.PasswordEntries.Any(
                entry => entry.Uuid == Uuid && entry.UserUuid == requestUuid
                )
            )
            {
                return BadRequest(new MessageDto("Password entry doesen't exist."));
            }
            _ = _dataContext.PasswordEntries
                .Remove(_dataContext.PasswordEntries
                    .First(entry => entry.Uuid == Uuid));
            _ = await _dataContext.SaveChangesAsync();
            return Ok(new MessageDto("Deleted the password."));
        }


        /// <summary>
        /// Returns all passwords from a user.
        /// </summary>
        /// <param></param>
        /// <returns PasswordDto="The passwords of the user."></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/ListPasswordEntrys
        ///     
        /// Sample response:
        /// 
        ///     [
        ///         {
        ///             "uuid": "3abbead3-4c20-4baf-94ae-03a45e239521",
        ///             "encryptedName": "Roblox",
        ///             "encryptedLink": "http://robux.com",
        ///             "encryptedUsername": "xXx_RobloxBoy187_xXx",
        ///             "encryptedPassword": "roblox11!!!111"
        ///         },
        ///         {
        ///             "uuid": "d4a10f01-9478-4c25-9a58-04e885789890",
        ///             "encryptedName": "Fortnite",
        ///             "encryptedLink": null,
        ///             "encryptedUsername": "FortnutFan34853847568934",
        ///             "encryptedPassword": "FoRtNiTeFoRlIfe_reeeeeeeeee"
        ///         }
        ///     ]
        /// </remarks>
        /// <response code="200">Returns all passwords.</response>
        [HttpGet("ListPasswordEntrys")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<PasswordDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PasswordDto>>> ListPasswordEntrys()
        {
            string? requestUuid = await Auth.ValidateAndGetUuid(_httpContextAccessor, _dataContext);
            return requestUuid == null
                ? (ActionResult<List<PasswordDto>>)BadRequest(new MessageDto("Invalid token."))
                : (ActionResult<List<PasswordDto>>)await Vault.ListPasswordEntrys(
                _dataContext,
                requestUuid
            );
        }
    }
}
