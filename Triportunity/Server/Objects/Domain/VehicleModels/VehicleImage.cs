using Server.Exceptions;
using System;

namespace Server.Objects.Domain.VehicleModels
{
    public class VehicleImage
    {
        public Guid Id { get; private set; }
        private double _size;

        public double Size
        {
            get => _size;
            set
            {
                if (value > 1000)
                {
                    throw new VehicleException("Size cannot be more than 1000 MB.");
                }
                _size = value;
            }
        }

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

        public VehicleImage(Guid id, string fileName, string fileExtension, string url, double size)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("ID cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new VehicleException("FileName cannot be null or whitespace.");
            }

            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                throw new VehicleException("FileExtension cannot be null or whitespace.");
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new VehicleException("URL cannot be null or whitespace.");
            }

            if (size <= 0 || size > 1000)
            {
                throw new VehicleException("Size must be positive and cannot be more than 1000 MB.");
            }

            Id = id;
            _fileName = fileName; 
            _fileExtension = fileExtension; 
            _url = url; 
            _size = size; 
        }
    }
}
