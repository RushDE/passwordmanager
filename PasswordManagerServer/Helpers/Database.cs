using Microsoft.EntityFrameworkCore;
using PasswordManagerServer.Data;

namespace PasswordManagerServer.Helpers
{
    /// <summary>
    /// Helper functions to manage the database.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Creates, and migrates the database.
        /// </summary>
        /// <param name="app"></param>
        public static void Create(WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            string? connectionString = dataContext.Database.GetConnectionString();
            if (connectionString == null)
            {
                Console.WriteLine("Error: No connection string specified.");
                return;

            }

            string? dbPath = connectionString.Replace("Data Source=", string.Empty);
            if (dbPath == null)
            {
                Console.WriteLine("Error: Couldn't find the database path.");
                return;
            }

            DirectoryInfo? dbDirectory = new FileInfo(dbPath).Directory;
            if (dbDirectory == null)
            {
                Console.WriteLine("Error: Couldn't find the database directory.");
                return;
            }

            _ = Directory.CreateDirectory(dbDirectory.FullName);
            dataContext.Database.Migrate();
        }
    }
}
