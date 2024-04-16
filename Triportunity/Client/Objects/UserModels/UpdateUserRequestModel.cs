using System;
using Client.Objects.UserModels;

namespace Client.Objects.ClientModels
{
    // This class is made for when the client wants to register as a driver.
    // So, at backend level it would generate for the client the specific DriverInfo
    public class UpdateUserRequestModel
    {
        public Guid ClientId { get; set; }

        public DriverInfo DriverAspects { get; set; }

        public UpdateUserRequestModel(Guid clientId, DriverInfo driverAspects)
        {
            ClientId = clientId;
            DriverAspects = driverAspects;

        }
    }
}