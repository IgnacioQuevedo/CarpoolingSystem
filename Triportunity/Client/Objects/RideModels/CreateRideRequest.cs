using System;
using System.Collections.Generic;
using Client.Objects.EnumsModels;
using Client.Objects.UserModels;


namespace Client.Objects.RideModels
{
    public class CreateRideRequest
    {
        public UserClient Driver { get; set; }
        public List<UserClient> Passengers { get; set; }
        public CitiesEnum InitialLocation { get; set; }
        public CitiesEnum EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public int TotalSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public string PhotoPath { get; set; }

        public CreateRideRequest(UserClient driver, List<UserClient> passengers, CitiesEnum initialLocation, CitiesEnum endingLocation, DateTime departureTime, int totalSeats, double pricePerPerson, bool petsAllowed, string photoPath)
        {
            Driver = driver;
            Passengers = passengers;
            InitialLocation = initialLocation;
            EndingLocation = endingLocation;
            DepartureTime = departureTime;
            TotalSeats = totalSeats;
            PricePerPerson = pricePerPerson;
            PetsAllowed = petsAllowed;
            PhotoPath = photoPath;
        }
    }
}
