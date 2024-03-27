using System;
using System.Collections.Generic;
using Serverg.Objects.Domain.ClientModels;

namespace Serverg.Objects.DTOs;

public class ModifyRideRequestDTO
{
    public string InitialLocation { get; set; }
    public string EndingLocation { get; set; }
    public DateTime DepartureTime { get; set; }
    public double PricePerPerson { get; set; }
    public bool PetsAllowed { get; set; }
    public string PhotoPath { get; set; }

    public ModifyRideRequestDTO(string initialLocation, string endingLocation, DateTime rideDate,
        DateTime departureTime, double pricePerPerson, bool petsAllowed, string photoPath)
    {
        InitialLocation = initialLocation;
        EndingLocation = endingLocation;
        DepartureTime = departureTime;
        PricePerPerson = pricePerPerson;
        PhotoPath = photoPath;
    }
}