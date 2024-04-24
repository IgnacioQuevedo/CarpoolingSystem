using System;
using Client.Objects.UserModels;

namespace Client.Objects.RideModels
{
    public class JoinRideRequest
    {
        public Guid RideId { get; set; }
        public UserClient PassengerToJoin { get; set; }

        public JoinRideRequest(Guid rideId, UserClient passengerToJoin)
        {
            RideId = Guid.Empty;
            PassengerToJoin = passengerToJoin;
        }
    }
}
