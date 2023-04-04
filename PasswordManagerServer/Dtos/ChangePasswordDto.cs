using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Dtos
{
    public class ChangePasswordDto
    {
        [Required]
        public string PrehashedOldPassword { get; set; } = string.Empty;
        [Required]
        public string PrehashedNewPassword { get; set; } = string.Empty;
    }
}
