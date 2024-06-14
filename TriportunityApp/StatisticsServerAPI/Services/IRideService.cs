using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.Services;

public interface IRideService
{
    public IEnumerable<RideEvent> GetRidesFiltered();
}