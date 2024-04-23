using System;
using Server.Objects.DTOs.VehicleImage;

namespace Server.Objects.DTOs.VehicleModelDto
{
    public class VehicleDto
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; }

        public VehicleDto(Guid id, string imagePath)
        {
            Id = id;
            ImagePath = imagePath;
        }
    }
}
