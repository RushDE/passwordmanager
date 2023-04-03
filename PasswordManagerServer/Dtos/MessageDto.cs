namespace PasswordManagerServer.Dtos
{
    public class MessageDto
    {
        public MessageDto(string message)
        {
            Message = message;
        }

        public string Message { get; set; } = string.Empty;
    }
}
