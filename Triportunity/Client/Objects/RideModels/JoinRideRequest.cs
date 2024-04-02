using ClientUI.Objects.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI.Objects.RideModels
{
    internal class JoinRideRequest
    {
        public Guid RideId { get; set; }
        public Client PassengerToJoin { get; set; }
    }
}
