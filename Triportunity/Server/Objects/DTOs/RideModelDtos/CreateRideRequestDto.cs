using System;
using System.Collections.Generic;
using Server.Objects.Domain.ClientModels;
using Server.Objects.Domain.Enums;
using Server.Objects.DTOs.ClientModelDtos;

namespace Server.Objects.DTOs.RideModelDtos
{

    public class CreateRideRequestDto
    {
        public Client Driver { get; set; }
        public List<ClientDto> Passengers { get; set; }
        public CitiesEnum InitialLocation { get; set; }
        public CitiesEnum EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public string PhotoPath { get; set; }

        public CreateRideRequestDto(Client driver, List<ClientDto> passengers, CitiesEnum initialLocation, CitiesEnum endingLocation,
            DateTime departureTime, int availableSeats, int totalSeats, double pricePerPerson, bool petsAllowed, string photoPath)
        {
            Driver = driver;
            Passengers = passengers;
            InitialLocation = initialLocation;
            EndingLocation = endingLocation;
            DepartureTime = departureTime;
            AvailableSeats = availableSeats;
            TotalSeats = totalSeats;
            PricePerPerson = pricePerPerson;
            PhotoPath = photoPath;
        }
    }
}