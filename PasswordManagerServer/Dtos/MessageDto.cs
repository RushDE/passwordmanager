using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Dtos
{
    /// <summary>
    /// The (returned) data transfer object for a general error or succes message.
    /// </summary>
    public class MessageDto
    {
        /// <summary>
        /// For easyer object creation.
        /// </summary>
        /// <param name="message"></param>
        public MessageDto(string message)
        {
            Message = message;
        }

        /// <summary>
        /// The succes or error message.
        /// </summary>
        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
