using StatisticsServerAPI.DataAccess.MemoryDatabase;
using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.DataAccess.Repositories;

public class RideRepository : IRideRepository
{
    public RideRepository()
    {
        
    }
    
    public void AddRideEvent(RideEvent rideEvent)
    {
        Database.GetInstance().RideEvents.Add(rideEvent);
    }

    public IEnumerable<RideEvent> GetRides()
    {
        return Database.GetInstance().RideEvents.ToList();
    }
}