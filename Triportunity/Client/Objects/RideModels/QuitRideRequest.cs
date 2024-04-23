using System;
using Client.Objects.UserModels;

namespace Client.Objects.RideModels
{
    public class QuitRideRequest
    {
        public Guid RideId { get; set; }
        public UserClient UserToExit { get; set; }
    }
}
