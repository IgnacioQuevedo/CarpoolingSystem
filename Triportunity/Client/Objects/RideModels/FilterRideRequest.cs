using ClientUI.Objects.EnumsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI.Objects.RideModels
{
    public class FilterRideRequest
    {
        public CitiesEnum EndingLocation { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
    }
}
