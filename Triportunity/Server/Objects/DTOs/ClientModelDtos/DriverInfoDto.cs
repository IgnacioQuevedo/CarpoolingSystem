using System.Collections.Generic;
using Server.Objects.DTOs.Review_Folder;
using Server.Objects.DTOs.Vehicle;

namespace Server.Objects.DTOs.ClientModelDtos
{
    public class DriverInfoDto
    {
        public int Ci { get; set; }
        public double Puntuation { get; set; }
        public ICollection<VehicleDto> Vehicles { get; set; }
        public ICollection<ReviewDto> Reviews { get; set; }

        public DriverInfoDto(int ci, ICollection<VehicleDto> vehicles)
        {
            Ci = ci;
            Vehicles = vehicles;
            Puntuation = 5.0;
            Reviews = new List<ReviewDto>();
        }
    }
}