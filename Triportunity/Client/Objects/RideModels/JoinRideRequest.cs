using System;
using Client.Objects.UserModels;

namespace Client.Objects.RideModels
{
    internal class JoinRideRequest
    {
        public Guid RideId { get; set; }
        public User PassengerToJoin { get; set; }
    }
}
