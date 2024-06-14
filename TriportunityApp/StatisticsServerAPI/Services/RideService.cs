using System.Reflection;
using StatisticsServerAPI.DataAccess.Repositories;
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
  
    public IEnumerable<GetRidesFilteredResponse> GetRidesFiltered(RideFilterDto filters)
    {
        IEnumerable<RideEvent> rides = _rideRepository.GetRides();
        
        foreach (PropertyInfo filterProperty in typeof(RideFilterDto).GetProperties())
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

        
        IEnumerable<GetRidesFilteredResponse> rideResponses = rides.Select(ride => new GetRidesFilteredResponse
        {
            Id = ride.Id,
            DriverId = ride.DriverId,
            Published = ride.Published,
            Passengers = ride.Passengers,
            InitialLocation = (CitiesEnumEventResponse) ride.InitialLocation,
            EndingLocation = (CitiesEnumEventResponse) ride.EndingLocation,
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


}