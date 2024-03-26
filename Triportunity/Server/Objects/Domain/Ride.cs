using Serverg.Objects.Domain.ClientModels;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;

namespace Server.Objects.Domain
{
    public class Ride
    {
        public Client Dirver { get; set; }
        public List<Client> Passengers { get; set; }
        public string InitialLocation { get; set; }
        public string EndingLocation { get; set; }
        public DateTime RideDate { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public string PhotoPath { get; set; }

    }
}