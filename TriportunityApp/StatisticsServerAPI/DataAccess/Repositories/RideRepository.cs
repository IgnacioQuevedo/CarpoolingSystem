using StatisticsServerAPI.DataAccess.MemoryDatabase;
using StatisticsServerAPI.Domain;
using StatisticsServerAPI.MqDomain;
using System.Collections.Concurrent;

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
        var reports = Database.GetInstance().RidesSummarizedReports
            .Where(report => report.AmountOfNextRidesToSummarize > 0)
            .ToList();

        foreach (var report in reports)
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