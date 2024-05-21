﻿using Server.Objects.DTOs.VehicleImage;
using System;

namespace Server.Objects.DTOs.VehicleModelDto
{
    public class CreateVehicleRequestDto
    {

        public CreateVehicleImageRequestDto  Picture { get; set; }

        public Guid Id { get; set; }
        public CreateVehicleRequestDto(Guid id, CreateVehicleImageRequestDto picture) {
            Id = id;
            Picture = picture;
        }
    }
}