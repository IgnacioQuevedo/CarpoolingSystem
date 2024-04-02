using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientUI.Objects.ClientModels;
using ClientUI.Objects.EnumsModels;

namespace ClientUI.Objects.RideModels
{
    public class CreateRideRequest
    {
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
    }
}
