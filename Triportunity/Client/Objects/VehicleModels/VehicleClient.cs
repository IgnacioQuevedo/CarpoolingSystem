using System;

namespace Client.Objects.VehicleModels
{
    public class VehicleClient
    {
        public Guid Id { get; set; }
        public string CarModel { get; set; }
        public string ImageAllocatedAtAServer { get; set; }

        public VehicleClient(Guid id, string carModel,string imageAllocatedAtAServer)
        {
            Id = id;
            CarModel = carModel;
            ImageAllocatedAtAServer = imageAllocatedAtAServer;
        }
    }
}