#nullable enable

namespace Client.Objects.UserModels
{
    public class RegisterUserRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string RepeatedPassword { get; set; }
        public DriverInfo? DriverAspects { get; set; }


        public RegisterUserRequest
            (string username, string password, string repeatedPassword, DriverInfo? driverAspects)
        {
            Username = username;
            Password = password;
            RepeatedPassword = repeatedPassword;
            DriverAspects = driverAspects;
        }
    }
}