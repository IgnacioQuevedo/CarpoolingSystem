using StatisticsServerAPI.Domain;
using StatisticsServerAPI.MqDomain;
using System.Collections.Concurrent;

namespace StatisticsServerAPI.DataAccess.MemoryDatabase;

public class Database
{
    public ConcurrentBag<LoginEvent> UserLoginEvents = new ConcurrentBag<LoginEvent>();
    public ConcurrentBag<RideEvent> RideEvents = new ConcurrentBag<RideEvent>();
    public ConcurrentBag<RidesSummarizedReport> RidesSummarizedReports = new ConcurrentBag<RidesSummarizedReport>();

    private static Database _database;
    private static readonly object singletonPadLock = new object();

    private Database()
    {
    }

    public static Database GetInstance()
    {
        lock (singletonPadLock)
        {
            if (_database is null)
            {
                _database = new Database();
            }

            return _database;
        }
    }
}