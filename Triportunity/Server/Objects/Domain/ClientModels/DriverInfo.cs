using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server.Exceptions;

namespace Server.Objects.Domain
{
    public class DriverInfo
    {
        public int Ci { get; set; }
        public double Puntuation { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }

        public DriverInfo(int ci,ICollection<Vehicle> driverVehicles)
        {
            Ci = ci;
            Puntuation = 5.0;
            Reviews = new List<Review>();
            Vehicles = driverVehicles;
            
            DriverInfoValidations();

        }

        public void DriverInfoValidations()
        {
            CheckIfCiIsEmpty();
        }

        private void CheckIfCiIsEmpty()
        {
            if (string.IsNullOrEmpty(Ci.ToString()))
            {
                throw new DriverInfoException("Ci must be declared.");
            }
        }
    }
}