namespace Client.Objects.ClientModels
{
    public class Client
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }


        public Client(string id,string username,string password)
        {
            Id = id;
            Username = username;
            Password = password;
        }
    }
}