using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Dtos
{
    public class JwtBearerDto
    {
        public JwtBearerDto(string message)
        {
            Message = message;
        }

        [Required]
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
    }
}
