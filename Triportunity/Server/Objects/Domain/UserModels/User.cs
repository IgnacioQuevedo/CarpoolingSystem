#nullable enable
using System;
using System.Text.RegularExpressions;
using Server.Exceptions;

namespace Server.Objects.Domain.UserModels
{
    public class User
    {
        public Guid Id { get; set; }
        public string Ci { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordRepeated { get; set; }
        public DriverInfo? DriverAspects { get; set; }

        public User(string ci, string username, string password, string passwordRepeated, DriverInfo? driverAspects)
        {
            Id = Guid.NewGuid();
            Username = username;
            Password = password;
            PasswordRepeated = passwordRepeated;
            Ci = ci;
            DriverAspects = driverAspects;

            ClientValidations();
        }

        private void ClientValidations()
        {
            UsernameValidation();
            PasswordValidation();
            CheckIfCiIsEmpty();
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
            
            if (Password != PasswordRepeated)
            {
                throw new UserException("Passwords must be the same!");
            }
            if (Password.Length < validLengthForPassword)
            {
                throw new UserException("Password length must be greater than: " + validLengthForPassword + "digits!");
            }
        }

        private void CheckIfCiIsEmpty()
        {
            int minimalLengthForCi = 6;

            if (string.IsNullOrEmpty(Ci))
            {
                throw new DriverInfoException("Ci must be declared.");
            }

            if (Ci.Length < minimalLengthForCi || !NumericFormatIsCorrect())
            {
                throw new DriverInfoException("Ci must be in a correct format. It must be at least of" +
                                              minimalLengthForCi + "and without special characters");
            }
        }

        private bool NumericFormatIsCorrect()
        {
            foreach (char c in Ci)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}