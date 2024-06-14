using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                RideFilterDto filtersToApply = new RideFilterDto
                {
                    PetsAllowed = filters.PetsAllowed,
                    PricePerPerson = filters.PricePerPerson,
                    AvailableSeats = filters.AvailableSeats
                };
                
                return Ok(_rideService.GetRidesFiltered(filtersToApply));
            }
            catch (Exception)
            {
                ObjectResult objectResult =
                    StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                return objectResult;
            }
        }
    }
}