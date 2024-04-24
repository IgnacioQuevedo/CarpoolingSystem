using System.Collections.Generic;
using Server.Exceptions;
using Server.Objects.Domain.VehicleModels;

namespace Server.Objects.Domain.ClientModels
{
    public class DriverInfo
    {
        public double Puntuation { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }

        public DriverInfo(ICollection<Vehicle> driverVehicles)
        {
            Puntuation = 5.0;
            Reviews = new List<Review>();
            Vehicles = driverVehicles;
            
            //DriverInfoValidations();
        }

        // private void DriverInfoValidations()
        // {
        //     ValidateThatExistsVehicles();
        // }

        // private void ValidateThatExistsVehicles()
        // {
        //     if (Vehicles.Count == 0)
        //     {
        //         throw new DriverInfoException("At least one vehicle must be declared");
        //     }
        // }

  
    }
}