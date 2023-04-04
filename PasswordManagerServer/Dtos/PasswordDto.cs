using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Dtos
{
    public class PasswordDto
    {
        public string? Uuid { get; set; }
        public string? EncryptedName { get; set; }
        public string? EncryptedLink { get; set; }
        public string? EncryptedUsername { get; set; }
        public string? EncryptedPassword { get; set; }
    }
}
