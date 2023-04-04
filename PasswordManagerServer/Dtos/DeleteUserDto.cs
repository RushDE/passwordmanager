using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Dtos
{
    public class DeleteUserDto
    {
        [Required]
        public string PrehashedPassword { get; set; } = string.Empty;
    }
}
