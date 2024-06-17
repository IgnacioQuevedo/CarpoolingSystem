using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StatisticsServerAPI.Domain;
using StatisticsServerAPI.Domain.Exceptions;
using StatisticsServerAPI.MqDomain;
using StatisticsServerAPI.Services;
using StatisticsServerAPI.WebModels.Requests;
using StatisticsServerAPI.WebModels.Responses;

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

        [HttpPost("filters")]
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
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost("reports")]
        public IActionResult CreateRidesSummarizedReport([FromBody] CreateRidesSummarizedReportRequest request)
        {
            try
            {
                CreateRidesSummarizedReportResponse creationOfReport =
                    _rideService.CreateRidesSummarizedReport(request.AmountOfNextRidesToSummarize);

                return Ok(creationOfReport);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("reports/{id:Guid}")]
        public IActionResult GetRidesSummarizedReport([FromRoute] Guid id)
        {
            try
            {
                RidesSummarizedReport report = _rideService.GetRidesSummarizedReportById(id);

                GetRidesSummarizedReportResponse reportResponse = new GetRidesSummarizedReportResponse
                {
                    RidesSummarized = report.RidesSummarized.Select(ride => new GetRideSummarizedResponse
                    {
                        Published = ride.Published,
                        Passengers = ride.Passengers,
                        InitialLocation = (CitiesEnumEventResponse)ride.InitialLocation,
                        DepartureTime = ride.DepartureTime,
                        AvailableSeats = ride.AvailableSeats,
                        PricePerPerson = ride.PricePerPerson
                    }).ToList()
                };

                return Ok(reportResponse);
            }
            catch (InvalidReportException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (NotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("completeness-of/reports/{id:Guid}")]
        public IActionResult CompletenessOfReport([FromRoute] Guid id)
        {
            try
            {
                bool isCompleted = _rideService.AskForCompleteness(id);

                if (isCompleted)
                {
                    return Accepted(new
                    {
                        message = "The report is done, you can now get it."
                    });
                }

                return Accepted(new
                {
                    message = "The report is not done yet, it needs more time."
                });
            }
            catch (NotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
