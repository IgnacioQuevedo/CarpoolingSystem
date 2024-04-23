#nullable enable
using Server.Objects.DTOs.UserModelDtos;

namespace Server.Objects.DTOs.ClientModelDtos
{
    public class RegisterUserRequestDto
    {
        public string Ci { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CreateDriverInfoRequestDto? DriverAspects { get; set; }

        public RegisterUserRequestDto
            (string ci, string username, string password, CreateDriverInfoRequestDto? driverAspects)
        {
            Ci = ci;
            Username = username;
            Password = password;
            DriverAspects = driverAspects;
        }
    }
}