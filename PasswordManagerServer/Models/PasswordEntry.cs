using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Models
{
    public class PasswordEntry
    {
        [Key]
        public string Uuid { get; set; } = string.Empty;
        public string UserUuid { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Link { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
