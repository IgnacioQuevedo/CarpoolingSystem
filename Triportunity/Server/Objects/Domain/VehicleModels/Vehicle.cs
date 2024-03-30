using Server.Exceptions;
using System;

namespace Server.Objects.Domain.VehicleModels
{
    public class Vehicle
    {
        public Guid Id { get; set; }

        public VehicleImage Picture { get; set; }

        public Vehicle(VehicleImage picture)
        {
            if (picture == null)
            {
                throw new VehicleException("The picture cannot be null.");
            }

            Id = Guid.NewGuid();
            Picture = picture;
        }
    }
}
