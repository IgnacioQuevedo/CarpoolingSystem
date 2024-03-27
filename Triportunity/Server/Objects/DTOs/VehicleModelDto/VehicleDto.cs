using System;
using Server.Objects.DTOs.VehicleImage;

namespace Server.Objects.DTOs.VehicleModelDto
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
