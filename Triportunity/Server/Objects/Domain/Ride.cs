using Server.Exceptions;
using Server.Objects.Domain.UserModels;
using Server.Objects.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Server.Objects.Domain
{
    public class Ride
    {
        public Guid Id { get; set; } 
        public Guid DriverId { get; set; }
        public bool Published { get; set; }
        public List<Guid> Passengers { get; set; }
        public CitiesEnum InitialLocation { get; set; }
        public CitiesEnum EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public Guid VehicleId { get; set; }

        public Ride() { }
        public Ride(Guid driver, CitiesEnum initialLocation, CitiesEnum endingLocation, DateTime departureTime, int availableSeats, double pricePerPerson, bool petsAllowed, Guid vehicleId)
        {
            DriverId = driver;
            Passengers = new List<Guid>();
            InitialLocation = initialLocation;
            EndingLocation = endingLocation;
            DepartureTime = departureTime;
            AvailableSeats = availableSeats;
            PricePerPerson = pricePerPerson;
            PetsAllowed = petsAllowed;
            VehicleId = vehicleId;
            Published = true;
            AvailableSeats = availableSeats;
            RideValidations();
        }
        private void RideValidations()
        {
            LocationValidator();
            SeatsValidator();
        }

        private void LocationValidator()
        {
            bool bothLocationsBelongToCitiesEnum = Enum.IsDefined(typeof(CitiesEnum), EndingLocation) && Enum.IsDefined(typeof(CitiesEnum), InitialLocation);

            if (!bothLocationsBelongToCitiesEnum)
            {
                throw new RideException("Locations must match with one city");
            }
        }

        private void SeatsValidator()
        {
            int minSeatsRequired = 1;

            if (AvailableSeats < minSeatsRequired)
            {
                throw new RideException("You must have at least " + minSeatsRequired + " seat available");
            }
        }

    }
}