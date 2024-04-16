#nullable enable
namespace Server.Objects.DTOs.ClientModelDtos
{
    public class RegisterUserRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string PasswordRepeated { get; set; }
        public CreateDriverInfoRequestDto? DriverAspects { get; set; }

        public RegisterUserRequestDto
            (string username, string password,string passwordRepeated, CreateDriverInfoRequestDto? driverAspects)
        {
            Username = username;
            Password = password;
            PasswordRepeated = passwordRepeated;
            DriverAspects = driverAspects;
        }
    }
}