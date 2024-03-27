using Server.Objects.DTOs.VehicleImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Objects.DTOs.Vehicle
{
    public class CreateVehicleResponseDto
    {
        public CreateVehicleImageResponseDto Picture { get; set; }

        public CreateVehicleResponseDto(CreateVehicleImageResponseDto picture)
        {
            Picture = picture;
        }
    }
}

