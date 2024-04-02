#nullable enable
using System;

namespace ClientUI.Objects.ClientModels
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }    
        public DriverInfo? DriverAspects { get; set; }
        
    }
}