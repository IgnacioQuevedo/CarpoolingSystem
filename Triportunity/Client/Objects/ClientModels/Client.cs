namespace Client.Objects
{
    public class Client
    {
        public string Username { get; set; }
        public string Password { get; set; }


        public Client(string username,string password)
        {
            Username = username;
            Password = password;
        }
    }
}