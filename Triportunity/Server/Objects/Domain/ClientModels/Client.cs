#nullable enable
using System;
using Server.Exceptions;
using Server.Objects.Domain;

namespace Serverg.Objects.Domain.ClientModels
{
    public abstract class Client
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DriverInfo? DriverAspects { get; set; }
        
        public Client(string username,string password, DriverInfo? driverAspects)
        {
            Id = new Guid();
            Username = username;
            Password = password;
            DriverAspects = driverAspects;
            
            UsernameValidation();
        }

        private void UsernameValidation()
        {
            const int validLengthForUsername = 3;

            if (Username.Length < validLengthForUsername)
            {
                throw new ClientException("Username length must be greater than 3 digits!");
            }
        }
    }
}