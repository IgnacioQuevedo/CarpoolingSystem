using Server.Objects.DTOs.VehicleImage;

namespace Server.Objects.DTOs.VehicleModelDto
{
    public class CreateVehicleRequestDto
    {

        public CreateVehicleImageRequestDto  Picture { get; set; }

        public CreateVehicleRequestDto(CreateVehicleImageRequestDto picture) {
            Picture = picture;
        }
    }
}
