using Microsoft.EntityFrameworkCore;
using PasswordManagerServer.Data;
using PasswordManagerServer.Dtos;
using PasswordManagerServer.Models;

namespace PasswordManagerServer.Helpers
{
    /// <summary>
    /// A helper class to reuse vault operations.
    /// </summary>
    public class Vault
    {
        /// <summary>
        /// Updates a password entry, but doesesnt commit the changes.
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="requestUuid"></param>
        /// <param name="passwordDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static void UpdatePasswordEntry(DataContext dataContext, string requestUuid, PasswordDto passwordDto)
        {
            if (string.IsNullOrWhiteSpace(passwordDto.Uuid))
            {
                throw new ArgumentException("The Uuid field has to be given.");
            }
            if (!dataContext.PasswordEntries.Any(
                entry => entry.Uuid == passwordDto.Uuid && entry.UserUuid == requestUuid
                )
            )
            {
                throw new ArgumentException("Password entry doesen't exist.");
            }
            PasswordEntry entry = dataContext.PasswordEntries.First(entry => entry.Uuid == passwordDto.Uuid);
            entry.Name = passwordDto.EncryptedName;
            entry.Link = passwordDto.EncryptedLink;
            entry.Username = passwordDto.EncryptedUsername;
            entry.Password = passwordDto.EncryptedPassword;
        }

        /// <summary>
        /// Gets all passwords from an user.
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="requestUuid"></param>
        /// <returns>All passwords from the given user.</returns>
        public static async Task<List<PasswordDto>> ListPasswordEntrys(DataContext dataContext, string requestUuid)
        {
            return (await dataContext.PasswordEntries
                .Where(entry => entry.UserUuid == requestUuid)
                .ToListAsync())
                .Select(
                    entry => new PasswordDto()
                    {
                        Uuid = entry.Uuid,
                        EncryptedName = entry.Name,
                        EncryptedLink = entry.Link,
                        EncryptedUsername = entry.Username,
                        EncryptedPassword = entry.Password
                    }
                )
                .ToList();
        }
    }
}