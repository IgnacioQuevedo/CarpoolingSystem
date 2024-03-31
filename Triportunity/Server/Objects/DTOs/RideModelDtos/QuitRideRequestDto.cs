using Server.Objects.Domain.ClientModels;
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
        public ClientDto ClientToExit { get; set; }

        public QuitRideRequestDto(Guid rideId, ClientDto clientToExit)
        {
            RideId = rideId;
            ClientToExit = clientToExit;
        }
    }
}
