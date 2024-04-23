using System;
using System.Security.Cryptography;
using Client.Objects.VehicleImageModels;

namespace Client.Objects.VehicleModels
{
    public class VehicleClient
    {
        public Guid Id { get; set; }

        public string ImageFileName { get; set; }

        public VehicleClient(Guid id, string ImageFileName)
        {
            Id = id;
            ImageFileName = ImageFileName;
        }
    }
}