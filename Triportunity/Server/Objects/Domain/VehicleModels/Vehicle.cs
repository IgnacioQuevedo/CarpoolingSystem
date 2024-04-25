using System;


namespace Server.Objects.Domain.VehicleModels
{
    public class Vehicle
    {
        public Guid Id { get; private set; }
        public string VehicleModel { get; set; }
        public string ImageAllocatedAtAServer { get; set; }

        public Vehicle() { }
        public Vehicle(string vehicleModel, string imageAllocatedAtAServer)
        {
            VehicleModel = vehicleModel;
            ImageAllocatedAtAServer = imageAllocatedAtAServer;
            Id = Guid.NewGuid();
        }
    }
}