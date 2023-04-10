using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Dtos
{
    /// <summary>
    /// The data transfer object for a user deletion.
    /// </summary>
    public class DeleteUserDto
    {
        /// <summary>
        /// The password should be prehashed once, with sha512 with the username as salt.
        /// </summary>
        [Required]
        public string PrehashedPassword { get; set; } = string.Empty;
    }
}
