using Server.Objects.DTOs.ClientModelDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Objects.DTOs.RideModelDtos
{
    public class QuitRideRequestDto
    {
        public Guid RideId { get; set; }
        public UserDto ClientToExit { get; set; }

        public QuitRideRequestDto(Guid rideId, UserDto clientToExit)
        {
            RideId = rideId;
            ClientToExit = clientToExit;
        }
    }
}
