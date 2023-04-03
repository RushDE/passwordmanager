using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Models
{
    public class PasswordEntry
    {
        [Key]
        public string Uuid { get; set; } = string.Empty;
        public string UserUuid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
