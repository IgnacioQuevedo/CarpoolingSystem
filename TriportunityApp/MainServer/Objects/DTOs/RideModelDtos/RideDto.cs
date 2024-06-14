using System;
using System.Collections.Generic;
using MainServer.Objects.Domain.Enums;
using MainServer.Objects.Domain.UserModels;

namespace MainServer.Objects.DTOs.RideModelDtos
{
    public class RideDto
    {
        public Guid Id { get; set; }
        public User Driver { get; set; }
        public List<User> Passengers { get; set; }
        public CitiesEnum InitialLocation { get; set; }
        public CitiesEnum EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public string PhotoPath { get; set; }

        public RideDto(Guid id, User driver, List<User> passengers, CitiesEnum initialLocation, CitiesEnum endingLocation, DateTime departureTime, int availableSeats, int totalSeats, double pricePerPerson, bool petsAllowed, string photoPath)
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