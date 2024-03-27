using System;

namespace Server.Objects.DTOs.RideModelDtos
{

    public class ModifyRideRequestDto
    {
        public string InitialLocation { get; set; }
        public string EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public string PhotoPath { get; set; }

        public ModifyRideRequestDto(string initialLocation, string endingLocation, DateTime rideDate,
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