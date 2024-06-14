using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.DataAccess.Repositories;

public interface IRideRepository
{
    public void AddRideEvent(RideEvent rideEvent);
    public IEnumerable<RideEvent> GetRides();
}