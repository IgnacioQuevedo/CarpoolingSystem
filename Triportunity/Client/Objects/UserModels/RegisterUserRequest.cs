
namespace Client.Objects.UserModels
{
    public class RegisterUserRequest
    {
        public string Ci { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RepeatedPassword { get; set; }
        public DriverInfoClient? DriverAspects { get; set; }


        public RegisterUserRequest
            (string ci,string username, string password, string repeatedPassword, DriverInfoClient? driverAspects)
        {
            Ci = ci;
            Username = username;
            Password = password;
            RepeatedPassword = repeatedPassword;
            DriverAspects = driverAspects;
        }
    }
}