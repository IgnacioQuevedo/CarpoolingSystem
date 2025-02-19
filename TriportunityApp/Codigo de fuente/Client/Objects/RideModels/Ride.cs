﻿using Client.Objects.EnumsModels;
using Client.Objects.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Objects.RideModels
{
    public class Ride
    {
        public Guid Id { get; set; }
        public UserClient Driver { get; set; }
        public List<UserClient> Passengers { get; set; }
        public CitiesEnum InitialLocation { get; set; }
        public CitiesEnum EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public string PhotoPath { get; set; }

    }
}
