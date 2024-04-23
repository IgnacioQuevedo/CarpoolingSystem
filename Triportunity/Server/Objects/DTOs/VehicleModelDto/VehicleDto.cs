using System;
using Server.Objects.DTOs.VehicleImage;

namespace Server.Objects.DTOs.VehicleModelDto
{
    public class VehicleDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }

        public VehicleDto(Guid id, string fileName)
        {
            Id = id;
            FileName = fileName;
        }
    }
}
