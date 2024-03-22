namespace Client.Objects.ClientModels
{
    public class LoginPassiveClientRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginPassiveClientRequest(string username,string password)
        {
            Username = username;
            Password = password;
        }
    }
}