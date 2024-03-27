using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serverg.Objects.Domain.ClientModels;

namespace Server.Objects.DTOs.Ride
{

    public class JoinRideRequestDTO
    {
        public Guid RideId { get; set; }
        public Client PassengerToJoin { get; set; }
    }
}

