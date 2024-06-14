using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StatisticsServerAPI.Services;

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
        public IActionResult GetRidesFiltered()
        {
            try
            {
                return Ok(_rideService.GetRidesFiltered());
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