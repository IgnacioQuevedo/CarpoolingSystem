using System;
using MainServer.Objects.DTOs.ClientModelDtos;

namespace MainServer.Objects.DTOs.RideModelDtos
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

