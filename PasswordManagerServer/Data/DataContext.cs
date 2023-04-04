using Microsoft.EntityFrameworkCore;
using PasswordManagerServer.Models;

namespace PasswordManagerServer.Data
{
    /// <summary>
    /// The data context for the database.
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="options"></param>
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

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
