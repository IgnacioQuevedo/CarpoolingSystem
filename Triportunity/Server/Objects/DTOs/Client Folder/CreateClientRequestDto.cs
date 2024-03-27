#nullable enable
using System.Collections.Generic;
using Server.Objects.Domain;

namespace Server.Objects.DTOs.Client
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