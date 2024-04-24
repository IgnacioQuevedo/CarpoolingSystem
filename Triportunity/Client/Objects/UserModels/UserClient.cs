#nullable enable
using System;

namespace Client.Objects.UserModels
{
    public class UserClient
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DriverInfoClient? DriverAspects { get; set; }

        public UserClient()
        {
            
        }

        public UserClient(string username, string password, DriverInfoClient? driverAspects)
        {
            Username = username;
            Password = password;
            DriverAspects = driverAspects;
        }
        public UserClient(Guid id, string username, string password, DriverInfoClient? driverAspects)
        {
            Id = id;
            Username = username;
            Password = password;
            DriverAspects = driverAspects;
        }
    }
}