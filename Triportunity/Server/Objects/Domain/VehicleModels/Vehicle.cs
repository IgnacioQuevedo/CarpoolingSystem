using System;


namespace Server.Objects.Domain.VehicleModels
{
    public class Vehicle
    {
        public Guid Id { get; private set; }
        public string FileName { get; set; }
        public string DestinationFilePath { get; set; }
    
       public Vehicle(string fileName,string destinationPath)
        {
            Id = Guid.NewGuid();
            FileName = fileName;
            DestinationFilePath = destinationPath;
        }
    }
}
