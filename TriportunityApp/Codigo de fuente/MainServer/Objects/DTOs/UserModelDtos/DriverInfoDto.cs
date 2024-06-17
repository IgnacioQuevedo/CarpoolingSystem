using System.Collections.Generic;
using MainServer.Objects.DTOs.ReviewModelDtos;
using MainServer.Objects.DTOs.VehicleModelDto;

namespace MainServer.Objects.DTOs.UserModelDtos
{
    public class DriverInfoDto
    {
        public double Puntuation { get; set; }
        public ICollection<VehicleDto> Vehicles { get; set; }
        public ICollection<ReviewDto> Reviews { get; set; }

        public DriverInfoDto(ICollection<ReviewDto> reviews,ICollection<VehicleDto> vehicles)
        {
            Puntuation = 5.0;
            Vehicles = vehicles;
            Reviews = reviews;
        }
    }
}