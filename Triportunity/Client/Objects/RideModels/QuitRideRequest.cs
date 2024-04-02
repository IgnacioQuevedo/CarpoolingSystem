using ClientUI.Objects.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI.Objects.RideModels
{
    public class QuitRideRequest
    {
        public Guid RideId { get; set; }
        public Client ClientToExit { get; set; }
    }
}
