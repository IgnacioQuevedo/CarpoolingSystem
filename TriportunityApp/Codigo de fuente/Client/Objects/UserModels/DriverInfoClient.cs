using System.Collections.Generic;
using Client.Objects.ReviewModels;
using Client.Objects.VehicleModels;

namespace Client.Objects.UserModels
{
    public class DriverInfoClient
    {
        public double Punctuation { get; set; } = 5.0;
        public ICollection<ReviewClient> Reviews { get; set; }
        public ICollection<VehicleClient> Vehicles { get; set; }

        //When creating
        public DriverInfoClient(ICollection<VehicleClient> vehicles)
        {
            Reviews = new List<ReviewClient>();
            Vehicles = vehicles;
        }

        //When login
        public DriverInfoClient(List<ReviewClient> reviews, List<VehicleClient> vehicles)
        {
            Reviews = reviews;
            Vehicles = vehicles;
        }
    }
}