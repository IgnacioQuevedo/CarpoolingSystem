using System;
using System.Collections.Generic;
using Server.Objects.Domain.Enums;
using Server.Objects.DTOs.ClientModelDtos;
using Server.Objects.DTOs.EnumsDtos;

namespace Server.Objects.DTOs.RideModelDtos
{

    public class CreateRideRequestDto
    {
        public UserDto Driver { get; set; }
        public List<UserDto> Passengers { get; set; }
        public CitiesEnumDto InitialLocation { get; set; }
        public CitiesEnumDto EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public string PhotoPath { get; set; }

        public CreateRideRequestDto(UserDto driver, List<UserDto> passengers, CitiesEnumDto initialLocation, CitiesEnumDto endingLocation,
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