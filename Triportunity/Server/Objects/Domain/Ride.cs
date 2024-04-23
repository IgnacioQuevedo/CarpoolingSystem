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
        public User Driver { get; set; }
        public bool Published { get; set; }
        public List<User> Passengers { get; set; }
        public CitiesEnum InitialLocation { get; set; }
        public CitiesEnum EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public string PhotoPath { get; set; }

        public Ride(User driver, List<User> passengers, CitiesEnum initialLocation, CitiesEnum endingLocation, DateTime departureTime, int availableSeats, int totalSeats, double pricePerPerson, bool petsAllowed, string photoPath)
        {
            Id = new Guid();
            Driver = driver;
            Passengers = passengers;
            InitialLocation = initialLocation;
            EndingLocation = endingLocation;
            DepartureTime = departureTime;
            AvailableSeats = availableSeats;
            TotalSeats = totalSeats;
            PricePerPerson = pricePerPerson;
            PetsAllowed = petsAllowed;
            PhotoPath = photoPath;
            Published = true;

            RideValidations();
        }

        private void RideValidations()
        {
            LocationValidator();
            SeatsValidator();
            PhotoPathValidator();
        }

        private void LocationValidator()
        {
            bool bothLocationsBelongToCitiesEnum = Enum.IsDefined(typeof(CitiesEnum), EndingLocation) && Enum.IsDefined(typeof(CitiesEnum), InitialLocation);

            if (bothLocationsBelongToCitiesEnum)
            {
                throw new RideException("Locations must match with one city");
            }
        }

        private void SeatsValidator()
        {
            int minSeatsRequired = 1;

            if (AvailableSeats < minSeatsRequired || TotalSeats < minSeatsRequired)
            {
                throw new RideException("You must have at least " + minSeatsRequired + " seat available");
            }
        }

        private void PhotoPathValidator()
        {
            if (PhotoPath.Length < 1)
            {
                throw new RideException("You must add a photo of your car");
            }
        }

    }
}