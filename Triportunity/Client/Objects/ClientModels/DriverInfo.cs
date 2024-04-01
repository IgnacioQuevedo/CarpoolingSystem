using System.Collections.Generic;
using Client.Objects.ReviewModels;
using Client.Objects.VehicleModels;

namespace Client.Objects.ClientModels
{
    public class DriverInfo
    {
        public string Ci { get; set; }
        public double Puntuation { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }

        public DriverInfo(string ci, ICollection<Vehicle>vehicles)
        {
            Ci = ci;
            Vehicles = vehicles;
        }
    }
}