#nullable enable
namespace Server.Objects.DTOs.ClientModelDtos
{
    public class CreateClientRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public CreateDriverInfoRequestDto? DriverAspects { get; set; }

        public CreateClientRequestDto(string username, string password, CreateDriverInfoRequestDto? driverAspects)
        {
            Username = username;
            Password = password;
            DriverAspects = driverAspects;
        }
    }
}