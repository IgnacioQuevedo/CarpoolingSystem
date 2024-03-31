using System;
using Server.Objects.Domain.ClientModels;
using Server.Objects.DTOs.ClientModelDtos;

namespace Server.Objects.DTOs.RideModelDtos
{

    public class JoinRideRequestDto
    {
        public Guid RideId { get; set; }
        public ClientDto PassengerToJoin { get; set; }

        public JoinRideRequestDto(Guid rideId, ClientDto passengerToJoin)
        {
            RideId = rideId;
            PassengerToJoin = passengerToJoin;
        }
    }
}

