using StatisticsServerAPI.Domain;
using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.DataAccess.Repositories;

public interface IRideRepository
{
    public void AddRideEvent(RideEvent rideEvent);
    public IEnumerable<RideEvent> GetRides();
    public void AddSummarizedReport(RidesSummarizedReport summarizedReport);
    public RidesSummarizedReport GetRidesSummarizedReportById(Guid id);
}