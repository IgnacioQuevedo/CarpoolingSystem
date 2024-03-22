using System.Collections;
using System.Collections.Generic;

namespace Server.Objects.Domain
{
    public class Driver : Client
    {

        public double Puntuation { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        
        
        public Driver(string username, string password) : base(username, password)
        {
        }
    }
}