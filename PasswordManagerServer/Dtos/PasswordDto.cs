namespace PasswordManagerServer.Dtos
{
    /// <summary>
    /// The data transfer object for everything with passwords.
    /// Everything except for the uuid should be encrypted on the client side,
    /// because of zero knoweledge.
    /// </summary>
    public class PasswordDto
    {
        /// <summary>
        /// The unique identifier of the password entry.
        /// </summary>
        public string? Uuid { get; set; }
        /// <summary>
        /// The account name.
        /// </summary>
        public string? EncryptedName { get; set; }
        /// <summary>
        /// The link to the webpage.
        /// </summary>
        public string? EncryptedLink { get; set; }
        /// <summary>
        /// The username for the account.
        /// </summary>
        public string? EncryptedUsername { get; set; }
        /// <summary>
        /// The password for the account.
        /// </summary>
        public string? EncryptedPassword { get; set; }
    }
}
