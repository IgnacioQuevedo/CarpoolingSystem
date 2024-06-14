using System.Collections.Generic;
using MainServer.Objects.DTOs.VehicleModelDto;

namespace MainServer.Objects.DTOs.UserModelDtos
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