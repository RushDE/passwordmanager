namespace PasswordManagerServer.Dtos
{
    public class JwtBearerDto
    {
        public JwtBearerDto(string message)
        {
            Message = message;
        }

        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
    }
}
