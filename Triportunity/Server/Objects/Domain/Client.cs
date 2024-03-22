using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Objects.Domain
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        public IEnumerable<Vehicle> Vehicles { get; set; }
        
        public Client(string username,string password)
        {
            Id = new Guid();
            Username = username;
            Password = password;
            Vehicles = new List<Vehicle>();
        }
    }
}