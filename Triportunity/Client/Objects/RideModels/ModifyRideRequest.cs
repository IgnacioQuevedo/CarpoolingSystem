using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Objects.EnumsModels;

namespace Client.Objects.RideModels
{
    public class ModifyRideRequest
    {
        public  Guid Id { get; set; }
        public CitiesEnum InitialLocation { get; set; }
        public CitiesEnum EndingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
        public string PhotoPath { get; set; }

        public ModifyRideRequest(CitiesEnum initialLocation, CitiesEnum endingLocation, DateTime departureTime, double pricePerPerson, bool petsAllowed, string photoPath)
        {
            InitialLocation = initialLocation;
            EndingLocation = endingLocation;
            DepartureTime = departureTime;
            PricePerPerson = pricePerPerson;
            PetsAllowed = petsAllowed;
            PhotoPath = photoPath;
        }
    }
}
