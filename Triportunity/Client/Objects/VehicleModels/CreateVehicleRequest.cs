using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
