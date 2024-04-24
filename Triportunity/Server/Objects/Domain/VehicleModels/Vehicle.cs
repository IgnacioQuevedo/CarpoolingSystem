using System;


namespace Server.Objects.Domain.VehicleModels
{
    public class Vehicle
    {
        public Guid Id { get; private set; }
        public string ImageAllocatedAtAServer { get; set; }
    
       public Vehicle()
        {
            Id = Guid.NewGuid();
        }
    }
}
