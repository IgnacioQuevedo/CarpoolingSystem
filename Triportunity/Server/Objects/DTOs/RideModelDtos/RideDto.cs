using System;
using System.Collections.Generic;
using Server.Objects.Domain.ClientModels;
using Server.Objects.Domain.Enums;

namespace Server.Objects.DTOs.RideModelDtos
{
    public class RideDto
    {
        public Guid Id { get; set; }
        public Client Driver { get; set; }
        public List<Client> Passengers { get; set; }
        public CitiesEnum InitialLocation { get; set; }
        public CitiesEnum EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public string PhotoPath { get; set; }

        public RideDto(Guid id, Client driver, List<Client> passengers, CitiesEnum initialLocation, CitiesEnum endingLocation, DateTime departureTime, int availableSeats, int totalSeats, double pricePerPerson, bool petsAllowed, string photoPath)
        {
            Id = id;
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