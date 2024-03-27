using Server.Objects.DTOs.VehicleImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Objects.DTOs.Vehicle
{
    public class CreateVehicleRequestDto
    {

        public CreateVehicleImageRequestDto  Picture { get; set; }

        public CreateVehicleRequestDto(CreateVehicleImageRequestDto picture) {
            Picture = picture;
        }
    }
}
