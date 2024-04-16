#nullable enable
using System;
using System.Text.RegularExpressions;
using Server.Exceptions;

namespace Server.Objects.Domain.ClientModels
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DriverInfo? DriverAspects { get; set; }

        public User(string username, string password, DriverInfo? driverAspects)
        {
            Id = Guid.NewGuid();
            Username = username;
            Password = password;
            DriverAspects = driverAspects;

            ClientValidations();
        }

        private void ClientValidations()
        {
            UsernameValidation();
            PasswordValidation();
        }

        private void UsernameValidation()
        {
            const int validLengthForUsername = 3;

            if (Username.Length < validLengthForUsername)
            {
                throw new UserException("Username length must be greater than: " + Password.Length + "digits!");
            }
        }

        private void PasswordValidation()
        {
            const int validLengthForPassword = 6;
            const int amountOfNumbersOnPassword = 4;
            string passwordRegularExpression = $@"^(?=(?:\D*\d){{{amountOfNumbersOnPassword},}})(?=.*[^\w\d\s])";

            if (Password.Length < validLengthForPassword ||
                !Regex.IsMatch(Password, passwordRegularExpression))
            {
                throw new UserException("Error on password. Password must be greater than: " +
                                          amountOfNumbersOnPassword + "and must contain: " + amountOfNumbersOnPassword +
                                          "numbers, and at least one special character!");
            }
        }
    }
}