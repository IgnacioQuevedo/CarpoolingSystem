namespace Client.Objects.ClientModels
{
    public class CreatePassiveClientRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }


        public CreatePassiveClientRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}