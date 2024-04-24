#nullable enable
using System;

namespace Client.Objects.UserModels
{
    public class UserClient
    {
        public Guid Id { get; set; }
        public string Ci { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DriverInfoClient? DriverAspects { get; set; }

        public UserClient()
        {
            
        }
        public UserClient(Guid id, string ci, string username, string password, DriverInfoClient? driverAspects)
        {
            Id = id;
            Ci = ci;
            Username = username;
            Password = password;
            DriverAspects = driverAspects;
        }
    }
}