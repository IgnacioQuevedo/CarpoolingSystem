#nullable enable
using System;

namespace Server.Objects.DTOs.ClientModelDtos
{
    public class ClientDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DriverInfoDto? DriverAspectsDto { get; set; }

        public ClientDto(Guid id, string username, string password, DriverInfoDto? driverAspectsDto)
        {
            Id = id;
            Username = username;
            Password = password;
            DriverAspectsDto = driverAspectsDto;

        }
    }
}