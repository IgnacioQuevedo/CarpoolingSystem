#nullable enable
using System.Collections.Generic;
using Server.Objects.Domain;

namespace Server.Objects.DTOs.Client
{
    public class CreateClientRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<VehicleDto>? Vehicles { get; set; }

        public CreateClientRequestDto(string username,string password, ICollection<VehicleDto> clientVehicles)
        {
            Username = username;
            Password = password;
            Vehicles = clientVehicles;
        }
    }
}