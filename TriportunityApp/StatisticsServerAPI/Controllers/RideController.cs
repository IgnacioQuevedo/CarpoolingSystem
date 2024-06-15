
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StatisticsServerAPI.Domain;
using StatisticsServerAPI.MqDomain;
using StatisticsServerAPI.Services;
using StatisticsServerAPI.WebModels.Requests;

namespace StatisticsServerAPI.Controllers
{
    [Route("api/v1/rides")]
    [ApiController]
    public class RideController : ControllerBase
    {
        private readonly IRideService _rideService;

        public RideController(IRideService rideService)
        {
            _rideService = rideService;
        }


        [HttpGet]
        public IActionResult GetRidesFiltered([FromBody] RideFilterRequest filters)
        {
            try
            {
                RideFilter filtersToApply = new RideFilter
                {
                    Id = filters.Id,
                    DriverId = filters.DriverId,
                    Published = filters.Published,
                    InitialLocation = filters.InitialLocation != null ? (CitiesEnumEvent?)filters.InitialLocation : null,
                    EndingLocation = filters.EndingLocation != null ? (CitiesEnumEvent?)filters.EndingLocation : null,
                    DepartureTime = filters.DepartureTime,
                    AvailableSeats = filters.AvailableSeats,
                    PricePerPerson = filters.PricePerPerson,
                    PetsAllowed = filters.PetsAllowed,
                    VehicleId = filters.VehicleId
                };


                
                return Ok(_rideService.GetRidesFiltered(filtersToApply));
            }
            catch (Exception exception)
            {
                ObjectResult objectResult =
                    StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                return objectResult;
            }
        }
    }
}