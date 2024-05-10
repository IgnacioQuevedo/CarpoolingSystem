﻿using System;
using Server.Exceptions;


namespace Server.Objects.Domain.VehicleModels
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string VehicleModel { get; set; }
        public string ImageAllocatedAtAServer { get; set; }
        
        public Vehicle(string vehicleModel, string imageAllocatedAtAServer)
        {
            Id = Guid.NewGuid();
            VehicleModel = vehicleModel;
            ImageAllocatedAtAServer = imageAllocatedAtAServer;
            VehicleValidations();
        }

        private void VehicleValidations()
        {
            if (string.IsNullOrEmpty(VehicleModel)) throw new VehicleException("Vehicle model must not be empty");
            if(string.IsNullOrEmpty(ImageAllocatedAtAServer)) throw new VehicleException("Vehicle image cannot be empty");
        }
        
    }
}