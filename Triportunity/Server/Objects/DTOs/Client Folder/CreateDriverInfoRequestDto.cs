using System;
using System.Collections.Generic;

namespace Server.Objects.DTOs.Client
{
    public class CreateDriverInfoRequestDto
    {
        public int Ci { get; set; }
        public ICollection<VehicleDto> Vehicles { get; set; }

        public CreateDriverInfoRequestDto(int ci, ICollection<VehicleDto> vehiclesOfClient)
        {
            Ci = ci;
            Vehicles = vehiclesOfClient;
        }
    }
}