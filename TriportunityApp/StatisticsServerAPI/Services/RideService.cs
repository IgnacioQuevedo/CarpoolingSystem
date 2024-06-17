using System.Reflection;
using StatisticsServerAPI.DataAccess.Repositories;
using StatisticsServerAPI.Domain;
using StatisticsServerAPI.Domain.Exceptions;
using StatisticsServerAPI.MqDomain;
using StatisticsServerAPI.WebModels.Requests;
using StatisticsServerAPI.WebModels.Responses;

namespace StatisticsServerAPI.Services;

public class RideService : IRideService
{
    private readonly IRideRepository _rideRepository;
    public RideService(IRideRepository rideRepository)
    {
        _rideRepository = rideRepository;
    }

    public IEnumerable<GetRideFilteredResponse> GetRidesFiltered(RideFilter filters)
    {
        IEnumerable<RideEvent> rides = _rideRepository.GetRides();

        foreach (PropertyInfo filterProperty in typeof(RideFilter).GetProperties())
        {
            object filterValue = filterProperty.GetValue(filters);

            if (filterValue != null)
            {
                // Check if RideEvent has the same property
                PropertyInfo rideProperty = typeof(RideEvent).GetProperty(filterProperty.Name);
                if (rideProperty != null)
                {
                    rides = ApplyFilter(rides, rideProperty, filterValue);
                }
            }
        }

        IEnumerable<GetRideFilteredResponse> rideResponses = rides.Select(ride => new GetRideFilteredResponse
        {
            Id = ride.Id,
            DriverId = ride.DriverId,
            Published = ride.Published,
            Passengers = ride.Passengers,
            InitialLocation = (CitiesEnumEventResponse)ride.InitialLocation,
            EndingLocation = (CitiesEnumEventResponse)ride.EndingLocation,
            DepartureTime = ride.DepartureTime,
            AvailableSeats = ride.AvailableSeats,
            PricePerPerson = ride.PricePerPerson,
            PetsAllowed = ride.PetsAllowed,
            VehicleId = ride.VehicleId
        });

        return rideResponses;
    }

    private IEnumerable<RideEvent> ApplyFilter(IEnumerable<RideEvent> rides, PropertyInfo rideProperty, object filterValue)
    {
        return rides.Where(ride =>
        {
            var ridePropertyValue = rideProperty.GetValue(ride);
            if (ridePropertyValue == null)
            {
                return false;
            }

            // Convert to strings for comparison if needed
            if (ridePropertyValue is IConvertible && filterValue is IConvertible)
            {
                return Convert.ToString(ridePropertyValue).Equals(Convert.ToString(filterValue));
            }

            // Handle other types of comparisons as needed
            return ridePropertyValue.Equals(filterValue);
        });
    }


    public CreateRidesSummarizedReportResponse CreateRidesSummarizedReport(int amountOfNextRidesToSummarize)
    {
        IEnumerable<RideEvent> ridesAtTheMoment = _rideRepository.GetRides();

        RidesSummarizedReport summarizedReport = new RidesSummarizedReport
        {
            AmountOfNextRidesToSummarize = amountOfNextRidesToSummarize,
        };

        _rideRepository.AddSummarizedReport(summarizedReport);

        return new CreateRidesSummarizedReportResponse
        {
            Id = summarizedReport.Id
        };
    }


    public bool AskForCompleteness(Guid id)
    {
        RidesSummarizedReport reportFound = _rideRepository.GetRidesSummarizedReportById(id);

        if (reportFound == null)
        {
            throw new NotFoundException($"RidesSummarizedReport with id {id} not found.");
        }

        bool isComplete = reportFound.AmountOfNextRidesToSummarize == 0;
        return isComplete;
    }



    public RidesSummarizedReport GetRidesSummarizedReportById(Guid idOfReportToGet)
    {
        RidesSummarizedReport reportFound = _rideRepository.GetRidesSummarizedReportById(idOfReportToGet);

        if (reportFound == null)
        {
            throw new NotFoundException($"RidesSummarizedReport with id {idOfReportToGet} not found.");
        }

        if (!AskForCompleteness(idOfReportToGet))
        {
            throw new InvalidReportException("Report is not done yet.");
        }

        return reportFound;
    }


}