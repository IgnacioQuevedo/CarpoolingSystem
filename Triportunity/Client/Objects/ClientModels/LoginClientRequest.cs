namespace ClientUI.Objects.ClientModels
{
    public class LoginClientRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginClientRequest(string username,string password)
        {
            Username = username;
            Password = password;
        }
    }
}