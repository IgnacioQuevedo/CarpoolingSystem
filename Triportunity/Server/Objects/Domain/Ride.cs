using Server.Exceptions;
using Server.Objects.Domain.ClientModels;
using System;
using System.Collections.Generic;

namespace Server.Objects.Domain
{
    public class Ride
    {
        public Guid Id { get; set; }
        public Client Driver { get; set; }
        public List<Client> Passengers { get; set; }
        public string InitialLocation { get; set; }
        public string EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public string PhotoPath { get; set; }

        public Ride(Client driver, List<Client> passengers, string initialLocation, string endingLocation, DateTime departureTime, int availableSeats, int totalSeats, double pricePerPerson, bool petsAllowed, string photoPath)
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
            int minLengthOfLocation = 1;

            if (EndingLocation.Length < minLengthOfLocation || InitialLocation.Length < minLengthOfLocation)
            {
                throw new RideException("Both locations must be filled");
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