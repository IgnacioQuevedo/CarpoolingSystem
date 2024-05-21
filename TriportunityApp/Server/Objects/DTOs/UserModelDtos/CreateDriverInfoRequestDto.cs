using System.Collections.Generic;
using Server.Objects.DTOs.VehicleModelDto;

namespace Server.Objects.DTOs.UserModelDtos
{
    public class CreateDriverInfoRequestDto
    {
        public ICollection<VehicleDto> Vehicles { get; set; }

        public CreateDriverInfoRequestDto(ICollection<VehicleDto> vehiclesOfClient)
        {
            Vehicles = vehiclesOfClient;
        }
    }
}