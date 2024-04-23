#nullable enable
using System;

namespace Server.Objects.DTOs.UserModelDtos
{
    public class GetUserResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public GetDriverInfoResponse? DriverAspectsDto { get; set; }

        public UserDto(Guid id, string username, string password, DriverInfoDto? driverAspectsDto)
        {
            Id = id;
            Username = username;
            Password = password;
            DriverAspectsDto = driverAspectsDto;

        }
    }

    public class GetDriverInfoResponse
    {
    }
}