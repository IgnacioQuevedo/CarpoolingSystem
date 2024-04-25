using System;
using Server.Objects.DTOs.ClientModelDtos;

namespace Server.Objects.DTOs.RideModelDtos
{

    public class JoinRideRequestDto
    {
        public Guid RideId { get; set; }
        public UserDto PassengerToJoin { get; set; }

        public JoinRideRequestDto(Guid rideId, UserDto passengerToJoin)
        {
            RideId = rideId;
            PassengerToJoin = passengerToJoin;
        }
    }
}

