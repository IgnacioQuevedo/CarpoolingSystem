#nullable enable
using System;
using MainServer.Objects.DTOs.UserModelDtos;

namespace MainServer.Objects.DTOs.ClientModelDtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DriverInfoDto? DriverAspectsDto { get; set; }

        public UserDto(Guid id, string username, string password, DriverInfoDto? driverAspectsDto)
        {
            Id = id;
            Username = username;
            Password = password;
            DriverAspectsDto = driverAspectsDto;

        }
    }
}