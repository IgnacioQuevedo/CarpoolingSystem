using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Objects.EnumsModels;

namespace Client.Objects.RideModels
{
    public class FilterRideRequest
    {
        public CitiesEnum EndingLocation { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }
    }
}
