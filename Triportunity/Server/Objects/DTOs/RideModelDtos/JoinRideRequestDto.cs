using System;
using Server.Objects.Domain.ClientModels;


namespace Server.Objects.DTOs.Ride_Folder
{

    public class JoinRideRequestDto
    {
        public Guid RideId { get; set; }
        public Client PassengerToJoin { get; set; }

        public JoinRideRequestDto(Guid rideId, Client passengerToJoin)
        {
            RideId = rideId;
            PassengerToJoin = passengerToJoin;
        }
    }
}

