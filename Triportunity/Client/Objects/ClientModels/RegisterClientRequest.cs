#nullable enable
using System.Collections.Generic;
using Client.Objects.VehicleModels;

namespace Client.Objects.ClientModels
{
    public class RegisterClientRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string RepeatedPassword { get; set; }
        public DriverInfo? DriverAspects { get; set; }


        public RegisterClientRequest
            (string username, string password, string repeatedPassword, DriverInfo? driverAspects)
        {
            Username = username;
            Password = password;
            RepeatedPassword = repeatedPassword;
            DriverAspects = driverAspects;
        }
    }
}