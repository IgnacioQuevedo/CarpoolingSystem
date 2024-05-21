using Server.Objects.Domain.Enums;
using System;

namespace Server.Objects.DTOs.RideModelDtos
{

    public class ModifyRideRequestDto
    {
        public CitiesEnum InitialLocation { get; set; }
        public CitiesEnum EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public string PhotoPath { get; set; }

        public ModifyRideRequestDto(CitiesEnum initialLocation, CitiesEnum endingLocation, DateTime rideDate,
            DateTime departureTime, double pricePerPerson, bool petsAllowed, string photoPath)
        {
            InitialLocation = initialLocation;
            EndingLocation = endingLocation;
            DepartureTime = departureTime;
            PricePerPerson = pricePerPerson;
            PhotoPath = photoPath;
        }
    }
}