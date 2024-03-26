using System;

namespace Server.Objects.Domain
{
    public class Vehicle
    {

        public Guid Id { get; set; }

        public VehicleImage Picture { get; set; }

        public Vehicle(VehicleImage picture)
        {
            Picture = picture;
        }
    }
}