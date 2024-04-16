using System.Collections.Generic;
using Server.Objects.DTOs.VehicleModelDto;

namespace Server.Objects.DTOs.ClientModelDtos
{
    public class CreateDriverInfoRequestDto
    {
        public int Ci { get; set; }
        public ICollection<VehicleDto> Vehicles { get; set; }

        public CreateDriverInfoRequestDto(int ci, ICollection<VehicleDto> vehiclesOfClient)
        {
            Ci = ci;
            Vehicles = vehiclesOfClient;
        }
    }
}