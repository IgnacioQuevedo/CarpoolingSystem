using System;
using System.Collections.Generic;
using Server.Objects.Domain;

namespace Server.Objects.DTOs.Client
{
    public class DriverInfoDto
    {
        public Guid Id { get; set; }
        public double Puntuation { get; set; }
        public ICollection<VehicleDto> Vehicles { get; set; }
        public ICollection<ReviewDto> Reviews { get; set; }

        public DriverInfoDto(Guid id, double puntuation, ICollection<VehicleDto> vehicles, ICollection<ReviewDto> reviews)
        {
            Id = id;
            Puntuation = puntuation;
            Vehicles = vehicles;
            Reviews = reviews;
        }
    }
}