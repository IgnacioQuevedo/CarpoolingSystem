namespace Client.Objects.UserModels
{
    public class LoginUserRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginUserRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}