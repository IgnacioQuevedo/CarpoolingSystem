namespace Client.Objects.ClientModels
{
    public class CreateClientRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }


        public CreateClientRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}