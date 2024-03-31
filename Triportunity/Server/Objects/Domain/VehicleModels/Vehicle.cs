using System;

namespace Server.Objects.Domain.VehicleModels
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public VehicleImage Picture { get; set; }

        public Vehicle(VehicleImage picture)
        {
            Id = new Guid();
            Picture = picture;
        }
    }
}