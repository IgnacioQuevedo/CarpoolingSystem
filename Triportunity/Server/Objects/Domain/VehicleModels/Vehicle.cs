using System;


namespace Server.Objects.Domain.VehicleModels
{
    public class Vehicle
    {
        public Guid Id { get; private set; }

        public string DestinationFilePath { get; set; }
    
       public Vehicle(string destinationPath)
        {
            Id = Guid.NewGuid();
            DestinationFilePath = destinationPath;
        }
    }
}
