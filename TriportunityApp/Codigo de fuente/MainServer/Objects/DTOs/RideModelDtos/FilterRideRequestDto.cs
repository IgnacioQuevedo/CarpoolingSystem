﻿using MainServer.Objects.Domain.Enums;
using MainServer.Objects.DTOs.EnumsDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer.Objects.DTOs.RideModelDtos
{
    public class FilterRideRequestDto
    {
        public CitiesEnumDto EndingLocation { get; set; }
        public double PricePerPerson { get; set; }
        public bool PetsAllowed { get; set; }

        public FilterRideRequestDto(CitiesEnumDto endingLocation, double pricePerPerson, bool petsAllowed)
        {
            EndingLocation = endingLocation;
            PricePerPerson = pricePerPerson;
            PetsAllowed = petsAllowed;
        }

    }
}
