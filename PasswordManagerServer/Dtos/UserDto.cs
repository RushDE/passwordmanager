using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Dtos
{
    /// <summary>
    /// The data transfer object for some user operations.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// The account username, should be unique.
        /// </summary>
        [Required]
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// The account password, should be prehashed once with sha256 with the username as salt.
        /// </summary>
        [Required]
        public string PrehashedPassword { get; set; } = string.Empty;
    }
}
