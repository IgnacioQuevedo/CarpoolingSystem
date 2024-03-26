namespace Server.Objects.Domain
{
    public class VehicleImage
    {

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string Url { get; set; }

        public VehicleImage(string fileName, string fileExtension, string url)
        {
            FileName = fileName;
            FileExtension = fileExtension;
            Url = url;
        }
    }
}