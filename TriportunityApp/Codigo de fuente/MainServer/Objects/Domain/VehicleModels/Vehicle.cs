using System;
using MainServer.Exceptions;


namespace MainServer.Objects.Domain.VehicleModels
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string VehicleModel { get; set; }
        public string ImageAllocatedAtServer { get; set; }
        
        public Vehicle(string vehicleModel)
        {
            Id = Guid.NewGuid();
            VehicleModel = vehicleModel;
            VehicleValidations();
        }
        
        private void VehicleValidations()
        {
            if (string.IsNullOrEmpty(VehicleModel)) throw new VehicleException("Vehicle model must not be empty");
        }
        
    }
}