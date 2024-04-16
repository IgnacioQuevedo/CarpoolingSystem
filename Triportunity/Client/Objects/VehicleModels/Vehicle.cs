using System;
using Client.Objects.VehicleImageModels;

namespace Client.Objects.VehicleModels
{
    public class Vehicle
    {

        public Guid Id { get; set; }
        public VehicleImage Picture { get; set; }

        public Vehicle(VehicleImage picture)
        {
            Id = Guid.NewGuid();
            Picture = picture;
        }
    }
}
