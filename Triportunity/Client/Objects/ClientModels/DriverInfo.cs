using System.Collections.Generic;
using ClientUI.Objects.ReviewModels;
using ClientUI.Objects.VehicleModels;

namespace ClientUI.Objects.ClientModels
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