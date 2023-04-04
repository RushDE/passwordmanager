using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Dtos
{
    public class UserDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PrehashedPassword { get; set; } = string.Empty;
    }
}
