using StatisticsServerAPI.DataAccess.MemoryDatabase;
using StatisticsServerAPI.Domain;
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
        AddRideIntoReports(rideEvent);
    }
    
    private void AddRideIntoReports(RideEvent rideEvent)
    {
        IEnumerable<RidesSummarizedReport> reports = Database.GetInstance().RidesSummarizedReports;

        foreach (var report in reports)
        {
            if (report.AmountOfNextRidesToSummarize > 0)
            {
                report.RidesSummarized.Add(new RidesSummarized
                {
                    Published = rideEvent.Published,
                    Passengers = rideEvent.Passengers,
                    InitialLocation = rideEvent.InitialLocation,
                    DepartureTime = rideEvent.DepartureTime,
                    AvailableSeats = rideEvent.AvailableSeats,
                    PricePerPerson = rideEvent.PricePerPerson
                });
                report.AmountOfNextRidesToSummarize--;
            }
        }
    }

    public IEnumerable<RideEvent> GetRides()
    {
        return Database.GetInstance().RideEvents.ToList();
    }

    public void AddSummarizedReport(RidesSummarizedReport summarizedReport)
    {
        Database.GetInstance().RidesSummarizedReports.Add(summarizedReport);
    }

    public RidesSummarizedReport GetRidesSummarizedReportById(Guid id)
    {
        return Database.GetInstance().RidesSummarizedReports.FirstOrDefault(x => x.Id == id);
    }
}