namespace PasswordManagerServer.Dtos
{
    public class ChangePasswordDto
    {
        public string PrehashedOldPassword { get; set; } = string.Empty;
        public string PrehashedNewPassword { get; set; } = string.Empty;
    }
}
