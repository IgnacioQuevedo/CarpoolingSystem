using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.DataAccess.Repositories;

public interface IRideEventRepository
{
    public void AddRideEvent(RideEvent rideEvent);
}