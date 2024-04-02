using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI.Objects.VehicleModels
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
