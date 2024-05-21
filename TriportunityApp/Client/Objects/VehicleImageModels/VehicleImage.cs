using System;

namespace Client.Objects.VehicleImageModels
{
    public class VehicleImage
    {
        public Guid Id { get; set; }

        public double Size { get; set; }
        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string Url { get; set; }

        public VehicleImage(Guid id, double size, string fileName, string fileExtension, string url)
        {
            Id = id;
            Size = size;
            FileName = fileName;
            FileExtension = fileExtension;
            Url = url;
        }
    }
}