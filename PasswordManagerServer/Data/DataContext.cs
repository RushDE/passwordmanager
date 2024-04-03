using Microsoft.EntityFrameworkCore;
using PasswordManagerServer.Models;

namespace PasswordManagerServer.Data
{
    /// <summary>
    /// The data context for the database.
    /// </summary>
    /// <remarks>
    /// The constructor.
    /// </remarks>
    /// <param name="options"></param>
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        /// <summary>
        /// All the account data.
        /// </summary>
        public DbSet<User> Users => Set<User>();
        /// <summary>
        /// All the passwords for the users.
        /// </summary>
        public DbSet<PasswordEntry> PasswordEntries => Set<PasswordEntry>();
    }
}
