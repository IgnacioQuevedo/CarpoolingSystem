using System;

namespace Client.Objects.VehicleModels
{
    public class VehicleClient
    {
        public Guid Id { get; set; }
        public string ImageFileName { get; set; }
        

        public VehicleClient(Guid id, string imageFileName)
        {
            Id = id;
            ImageFileName = imageFileName;
        }
    }
}