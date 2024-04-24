using System;

namespace Client.Objects.VehicleModels
{
    public class VehicleClient
    {
        public Guid Id { get; set; }
        public string CarModel { get; set; }

        public VehicleClient(Guid id, string carModel)
        {
            Id = id;
            CarModel = carModel;
        }
    }
}