﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Objects.DTOs.VehicleImage
{
    public class CreateVehicleImageResponseDto
    {
            public string Url { get; set; }

            public string FileName { get; set; }

            public string FileExtension { get; set; }

            public CreateVehicleImageResponseDto(string url, string fileExtension, string fileName)
            {
                Url = url;
                FileName = fileName;
                FileExtension = fileExtension;
            }
        }
    }
}
