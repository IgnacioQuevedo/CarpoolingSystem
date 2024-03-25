using System;

namespace Server.Objects.DTOs.Client
{
    public class ClientDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ClientDto(string username, string password)
        {
            Id = new Guid();
            Username = username;
            Password = password;
        }
    }
}