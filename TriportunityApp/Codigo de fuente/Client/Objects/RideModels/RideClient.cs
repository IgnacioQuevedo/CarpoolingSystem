﻿using Client.Objects.EnumsModels;
using Client.Objects.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Objects.VehicleModels;

namespace Client.Objects.RideModels
{
    public class RideClient
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public Guid VehicleId { get; set; }
        public ICollection<Guid> Passengers { get; set; }
        public CitiesEnum InitialLocation { get; set; }
        public CitiesEnum EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }

    }
}