using System;
using Client.Objects.UserModels;

namespace Client.Objects.RideModels
{
    public class JoinRideRequest
    {
        public Guid RideId { get; set; }
        public User PassengerToJoin { get; set; }

        public JoinRideRequest(Guid rideId, User passengerToJoin)
        {
            RideId = Guid.Empty;
            PassengerToJoin = passengerToJoin;
        }
    }
}
