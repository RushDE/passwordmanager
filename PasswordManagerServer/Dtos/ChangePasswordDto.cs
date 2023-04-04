using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Dtos
{
    /// <summary>
    /// The data transfer object for a password change.
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// The old password of the user, prehashed once with bcrypt.
        /// </summary>
        [Required]
        public string PrehashedOldPassword { get; set; } = string.Empty;
        /// <summary>
        /// The new password of the user, prehashed once with bcrypt.
        /// </summary>
        [Required]
        public string PrehashedNewPassword { get; set; } = string.Empty;
    }
}
