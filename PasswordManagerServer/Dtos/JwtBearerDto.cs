using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Dtos
{
    /// <summary>
    /// The (returnded) data transfer object for a login.
    /// </summary>
    /// <remarks>
    /// For easyer object creation.
    /// </remarks>
    /// <param name="message"></param>
    public class JwtBearerDto(string message)
    {
        /// <summary>
        /// The succes or error message.
        /// </summary>
        [Required]
        public string Message { get; set; } = message;
        /// <summary>
        /// The jwt bearer token.
        /// </summary>
        public string? Token { get; set; }
    }
}
