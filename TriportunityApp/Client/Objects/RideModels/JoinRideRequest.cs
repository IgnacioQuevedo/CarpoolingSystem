using System;
using Client.Objects.UserModels;

namespace Client.Objects.RideModels
{
    public class JoinRideRequest
    {
        public Guid RideId { get; set; }
        public Guid PassengerToJoin { get; set; }

        public JoinRideRequest(Guid rideId, Guid passengerToJoin)
        {
            RideId = rideId;
            PassengerToJoin = passengerToJoin;
        }
    }
}
