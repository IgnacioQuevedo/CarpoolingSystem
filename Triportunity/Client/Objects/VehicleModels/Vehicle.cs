﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Objects.VehicleModels
{
    public class Vehicle
    {

        public Guid Id { get; set; }

        public VehicleImage Picture { get; set; }

        public Vehicle(VehicleImage picture)
        {
            Id = new Guid();
            Picture = picture;
        }
    }
}