using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Dtos
{
    /// <summary>
    /// The (returnded) data transfer object for a login.
    /// </summary>
    public class JwtBearerDto
    {
        /// <summary>
        /// For easyer object creation.
        /// </summary>
        /// <param name="message"></param>
        public JwtBearerDto(string message)
        {
            Message = message;
        }

        /// <summary>
        /// The succes or error message.
        /// </summary>
        [Required]
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// The jwt bearer token.
        /// </summary>
        public string? Token { get; set; }
    }
}
