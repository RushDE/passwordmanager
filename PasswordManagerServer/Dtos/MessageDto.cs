using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Dtos
{
    public class MessageDto
    {
        public MessageDto(string message)
        {
            Message = message;
        }

        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
