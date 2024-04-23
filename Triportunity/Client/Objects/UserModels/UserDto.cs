#nullable enable
using System;

namespace Client.Objects.UserModels
{
    public class UserDto
    {
        public Guid Id { get; set; } 
        public string Username { get; set; }
        public string Password { get; set; }    
        public DriverInfo? DriverAspects { get; set; }
        
    }
}