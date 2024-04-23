using System.Collections.Generic;
using Server.Objects.DTOs.ReviewModelDtos;
using Server.Objects.DTOs.VehicleModelDto;

namespace Server.Objects.DTOs.UserModelDtos
{
    public class GetDriverInfoResponse
    {
        public double Puntuation { get; set; }
        public ICollection<ReviewDto> Reviews { get; set; }
        public ICollection<VehicleDto> Vehicles { get; set; }

        public GetDriverInfoResponse(double punctuation ,ICollection<ReviewDto> reviews, ICollection<VehicleDto> vehicles)
        {
            Puntuation = punctuation;
            Vehicles = vehicles;
            Reviews = reviews;
        }
    }
}