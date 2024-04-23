using System.Collections.Generic;
using Client.Objects.ReviewModels;
using Client.Objects.VehicleModels;

namespace Client.Objects.UserModels
{
    public class DriverInfoClient
    {
        public double Punctuation { get; set; }
        public ICollection<ReviewClient> Reviews { get; set; }
        public ICollection<VehicleClient> Vehicles { get; set; }

        public DriverInfoClient(double punctuation, ICollection<ReviewClient> reviews, ICollection<VehicleClient> vehicles)
        {
            Punctuation = punctuation;
            Reviews = reviews;
            Vehicles = vehicles;
        }
    }
}