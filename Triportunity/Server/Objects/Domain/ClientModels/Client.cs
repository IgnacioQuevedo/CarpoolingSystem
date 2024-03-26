#nullable enable
using System;
using System.Text.RegularExpressions;
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
                throw new ClientException("Username length must be greater than: " + Password.Length + "digits!");
            }
        }

        private void PasswordValidation()
        {
            const int validLengthForPassword = 6;
            const int amountOfNumbersOnPassword = 4;
            if (Password.Length < 6)
            {
                throw new ClientException("Password must be greater than: " + Password.Length + "digits");
            }
            
            // Expresión regular que busca al menos 4 números y un símbolo especial
            ValidatePasswordContainsXNumbersAndSpecialCharacter(amountOfNumbersOnPassword);
        }

        private void ValidatePasswordContainsXNumbersAndSpecialCharacter(int amountOfNumbers)
        {
            string pattern = $@"^(?=(?:\D*\d){{{amountOfNumbers},}})(?=.*[^\w\d\s])";
            
            if (!Regex.IsMatch(Password, pattern))
            {
                throw new ClientException("Password must contain: " + amountOfNumbers + 
                                          "digits and at least an special character!");
            }
        }
    }
}