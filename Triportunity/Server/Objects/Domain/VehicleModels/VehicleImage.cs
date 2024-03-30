using Server.Exceptions;
using System;

namespace Server.Objects.Domain.VehicleModels
{
   public class VehicleImage
        {
            private string _fileName;
            public string FileName
            {
                get => _fileName;
                set => _fileName = !string.IsNullOrWhiteSpace(value) ? value : throw new VehicleException("Value cannot be null or whitespace.");
            }

            private string _fileExtension;
            public string FileExtension
            {
                get => _fileExtension;
                set => _fileExtension = !string.IsNullOrWhiteSpace(value) ? value : throw new VehicleException("Value cannot be null or whitespace.");
            }

            private string _url;
            public string Url
            {
                get => _url;
                set => _url = !string.IsNullOrWhiteSpace(value) ? value : throw new VehicleException("Value cannot be null or whitespace.");
            }

            public VehicleImage(string fileName, string fileExtension, string url)
            {
                FileName = fileName;
                FileExtension = fileExtension;
                Url = url;
            }
        }

    }

