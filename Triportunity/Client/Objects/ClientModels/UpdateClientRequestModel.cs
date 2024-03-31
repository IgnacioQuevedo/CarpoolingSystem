using System;
using System.Collections.Generic;
using Client.Objects.VehicleModels;

namespace Client.Objects.ClientModels
{
    public class UpdateClientRequestModel
    {
        public Guid Id { get; set; }
        public int Ci { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}