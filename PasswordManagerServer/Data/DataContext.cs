using Microsoft.EntityFrameworkCore;
using PasswordManagerServer.Models;

namespace PasswordManagerServer.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<PasswordEntry> PasswordEntries => Set<PasswordEntry>();
    }
}
