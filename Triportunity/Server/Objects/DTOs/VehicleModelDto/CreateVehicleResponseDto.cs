using Server.Objects.DTOs.VehicleImage;

namespace Server.Objects.DTOs.VehicleModelDto
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

