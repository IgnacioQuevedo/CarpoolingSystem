using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Server.Objects.Domain
{
    public class DriverInfo
    {
        public Guid Id { get; set; } = new Guid();
        public int Ci { get; set; }
        public double Puntuation { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }

        public DriverInfo(int ci,ICollection<Vehicle> driverVehicles)
        {
            Ci = ci;
            Puntuation = 5.0;
            Reviews = new List<Review>();
            Vehicles = new List<Vehicle>();

            for (int i = 0; i < driverVehicles.Count(); i++)
            {
                Vehicles.Add(driverVehicles.ElementAtOrDefault(i));
            }

        }
    }
}