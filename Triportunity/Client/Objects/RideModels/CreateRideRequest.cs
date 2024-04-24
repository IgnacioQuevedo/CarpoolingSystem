using System;
using System.Collections.Generic;
using Client.Objects.EnumsModels;
using Client.Objects.UserModels;
using Client.Objects.VehicleModels;


namespace Client.Objects.RideModels
{
    public class CreateRideRequest
    {
        public UserClient Driver { get; set; }
        public Guid VehicleId { get; set; }
        public List<UserClient> Passengers { get; set; }
        public CitiesEnum InitialLocation { get; set; }
        public CitiesEnum EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public int TotalSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }

        public CreateRideRequest(UserClient driver,Guid vehicleId, List<UserClient> passengers, CitiesEnum initialLocation, CitiesEnum endingLocation, DateTime departureTime, int totalSeats, double pricePerPerson, bool petsAllowed, string photoPath)
        {
            Driver = driver;
            Passengers = passengers;
            InitialLocation = initialLocation;
            EndingLocation = endingLocation;
            DepartureTime = departureTime;
            TotalSeats = totalSeats;
            PricePerPerson = pricePerPerson;
            PetsAllowed = petsAllowed;
            VehicleId = vehicleId;
        }
    }
}
