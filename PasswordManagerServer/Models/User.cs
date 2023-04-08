using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Models
{
    /// <summary>
    /// The user table for the database.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The unique user identifier.
        /// </summary>
        [Key]
        public string Uuid { get; set; } = string.Empty;
        /// <summary>
        /// The account username.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// The password hash for the account, teeice hashed with sha256 with the username as salt.
        /// Once on the client side, once on the server side.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;
    }
}
