using Server.Objects.DTOs.VehicleImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Objects.DTOs.Vehicle
{
    public class VehicleDto
    {
        public Guid Id { get; set; }

        public VehicleImageDto Picture { get; set; }

        public VehicleDto(Guid id, VehicleImageDto picture)
        {
            Id = id;
            Picture = picture;
        }
    }
}
