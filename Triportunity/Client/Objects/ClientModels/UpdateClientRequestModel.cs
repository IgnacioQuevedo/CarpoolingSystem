using System;
using System.Collections.Generic;
using ClientUI.Objects.VehicleModels;

namespace ClientUI.Objects.ClientModels
{
    // This class is made for when the client wants to register as a driver.
    // So, at backend level it would generate for the client the specific DriverInfo
    public class UpdateClientRequestModel
    {
        public Guid Id { get; set; }
        public int Ci { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}