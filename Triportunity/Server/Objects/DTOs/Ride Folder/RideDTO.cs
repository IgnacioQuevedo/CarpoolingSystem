using Serverg.Objects.Domain.ClientModels;
using System.Collections.Generic;
using System;

namespace Server.Objects.DTOs
{
    public class RideDTO
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

        public RideDTO(Client driver, List<Client> passengers, string initialLocation, string endingLocation, DateTime departureTime, int availableSeats, int totalSeats, double pricePerPerson, bool petsAllowed, string photoPath)
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
        }
    }
}