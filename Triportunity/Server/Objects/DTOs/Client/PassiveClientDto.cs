using System;

namespace Server.Objects.DTOs.Client
{
    public class PassiveClientDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public PassiveClientDto(Guid id, string username, string password)
        {
            Id = id;
            Username = username;
            Password = password;
        }
    }
}