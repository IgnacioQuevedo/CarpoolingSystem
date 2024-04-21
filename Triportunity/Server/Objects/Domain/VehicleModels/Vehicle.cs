using Server.Exceptions;
using System;

namespace Server.Objects.Domain.VehicleModels
{
    public class Vehicle
    {
        public Guid Id { get; private set; } 

        private VehicleImage _picture;
        public VehicleImage Picture
        {
            get => _picture;
            set
            {
                if (value == null)
                {
                    throw new VehicleException("The picture cannot be null.");
                }
                _picture = value;
            }
        }
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
