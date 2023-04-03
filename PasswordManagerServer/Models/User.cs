namespace PasswordManagerServer.Models
{
    public class User
    {
        public string Uuid { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
