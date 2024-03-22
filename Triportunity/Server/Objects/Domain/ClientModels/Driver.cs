using System.Collections.Generic;
using Serverg.Objects.Domain.ClientModels;

namespace Server.Objects.Domain.ClientModels
{
    public class Driver : Client
    {

        public double Puntuation { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public IEnumerable<Vehicle> Vehicles { get; set; }
        
        
        public Driver(string username, string password,double puntuation) : base(username, password)
        {
            Puntuation = puntuation;
            Reviews = new List<Review>();
            Vehicles = new List<Vehicle>();
        }
    }
}