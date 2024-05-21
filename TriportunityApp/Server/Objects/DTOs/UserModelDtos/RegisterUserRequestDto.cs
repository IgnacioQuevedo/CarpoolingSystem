#nullable enable
namespace Server.Objects.DTOs.UserModelDtos
{
    public class RegisterUserRequestDto
    {
        public string Ci { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordRepeated { get; set; }
        public CreateDriverInfoRequestDto? DriverAspects { get; set; }

        public RegisterUserRequestDto
            (string ci, string username, string password,string passwordRepeated, CreateDriverInfoRequestDto? driverAspects)
        {
            Ci = ci;
            Username = username;
            Password = password;
            PasswordRepeated = passwordRepeated;
            DriverAspects = driverAspects;
        }
    }
}