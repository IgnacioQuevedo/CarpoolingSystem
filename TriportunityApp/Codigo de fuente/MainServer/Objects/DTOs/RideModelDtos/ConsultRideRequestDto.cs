using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer.Objects.DTOs.RideModelDtos
{
    public class ConsultRideRequestDto
    {
        public Guid RideId { get; set; }

        public ConsultRideRequestDto(Guid rideId)
        {
            RideId = rideId;
        }
    }
}
