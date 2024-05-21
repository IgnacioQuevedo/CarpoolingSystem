using System;
using Client.Objects.VehicleImageModels;

namespace Client.Objects.VehicleModels
{
    public class CreateVehicleRequest
    {
        public Guid Id { get; set; }

        public VehicleImage Picture { get; set; }


        public CreateVehicleRequest(Guid id, VehicleImage picture)
        {
            Id = id;
            Picture = picture;
        }
    }
}
