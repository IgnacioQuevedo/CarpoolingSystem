using StatisticsServerAPI.DataAccess.MemoryDatabase;
using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.DataAccess.Repositories;

public class RideEventRepository : IRideEventRepository
{
    public RideEventRepository()
    {
        
    }
    
    public void AddRideEvent(RideEvent rideEvent)
    {
        Database.GetInstance().RideEvents.Add(rideEvent);
    }
}