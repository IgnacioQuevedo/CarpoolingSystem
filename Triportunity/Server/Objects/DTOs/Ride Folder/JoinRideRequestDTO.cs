using System;
using Server.Objects.Domain.ClientModels;


namespace Server.Objects.DTOs.Ride_Folder
{

    public class JoinRideRequestDTO
    {
        public Guid RideId { get; set; }
        public Client PassengerToJoin { get; set; }

        public JoinRideRequestDTO(Guid rideId, Client passengerToJoin)
        {
            RideId = rideId;
            PassengerToJoin = passengerToJoin;
        }
    }
}

