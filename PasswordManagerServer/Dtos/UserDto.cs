namespace PasswordManagerServer.Dtos
{
    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
        public string PrehashedPassword { get; set; } = string.Empty;
    }
}
