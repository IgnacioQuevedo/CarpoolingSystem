﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Objects.EnumsModels;

namespace Client.Objects.RideModels
{
    public class ModifyRideRequest
    {
        public  Guid RideId { get; set; }
        public CitiesEnum InitialLocation { get; set; }
        public CitiesEnum EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public Guid VehicleId { get; set; }
        public Guid DriverId { get; set; }

        public ModifyRideRequest(Guid rideId, CitiesEnum initialLocation, CitiesEnum endingLocation, DateTime departureTime, double pricePerPerson, bool petsAllowed, Guid vehicleId, int availableSeats, Guid driverId)
        {
            RideId = rideId;
            InitialLocation = initialLocation;
            EndingLocation = endingLocation;
            DepartureTime = departureTime;
            PricePerPerson = pricePerPerson;
            PetsAllowed = petsAllowed;
            VehicleId = vehicleId;
            AvailableSeats = availableSeats;
            DriverId = driverId;
        }
    }
}
