using System;

namespace Server.Objects.Domain
{
    public abstract class Client
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        public Client(string username,string password)
        {
            Id = new Guid();
            Username = username;
            Password = password;
        }
    }
}