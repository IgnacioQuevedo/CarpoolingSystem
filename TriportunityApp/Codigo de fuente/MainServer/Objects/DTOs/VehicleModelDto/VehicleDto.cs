using System;
using MainServer.Objects.DTOs.VehicleImage;

namespace MainServer.Objects.DTOs.VehicleModelDto
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
