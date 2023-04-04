using System.ComponentModel.DataAnnotations;

namespace PasswordManagerServer.Models
{
    /// <summary>
    /// The password entry table for the database.
    /// </summary>
    public class PasswordEntry
    {
        /// <summary>
        /// The unique identifier of a password entry.
        /// </summary>
        [Key]
        public string Uuid { get; set; } = string.Empty;
        /// <summary>
        /// The uuid of the owner.
        /// </summary>
        public string UserUuid { get; set; } = string.Empty;
        /// <summary>
        /// The account name.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// The link to the webpage.
        /// </summary>
        public string? Link { get; set; }
        /// <summary>
        /// The username for the account.
        /// </summary>
        public string? Username { get; set; }
        /// <summary>
        /// The password for the account.
        /// </summary>
        public string? Password { get; set; }
    }
}
