using System;
using System.Collections.Generic;
using Client.Objects.EnumsModels;
using Client.Objects.UserModels;
using Client.Objects.VehicleModels;


namespace Client.Objects.RideModels
{
    public class CreateRideRequest
    {
        public Guid DriverId { get; set; }
        public Guid VehicleId { get; set; }
        public List<Guid> Passengers { get; set; }
        public CitiesEnum InitialLocation { get; set; }
        public CitiesEnum EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }


        public CreateRideRequest(Guid driver, List<Guid> passengers, 
            CitiesEnum initialLocation, CitiesEnum endingLocation, DateTime departureTime, int availableSeats, double pricePerPerson, bool petsAllowed, Guid vehicleId)
        {
            DriverId = driver;
            Passengers = passengers;
            InitialLocation = initialLocation;
            EndingLocation = endingLocation;
            DepartureTime = departureTime;
            AvailableSeats = availableSeats;
            PricePerPerson = pricePerPerson;
            PetsAllowed = petsAllowed;
            VehicleId = vehicleId;
        }
    }
}
