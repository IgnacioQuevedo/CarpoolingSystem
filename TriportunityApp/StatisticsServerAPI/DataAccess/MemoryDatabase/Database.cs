using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.DataAccess.MemoryDatabase;

public class Database 
{
    public ICollection<LoginEvent> UserLoginEvents = new List<LoginEvent>();
    public ICollection<RideEvent> RideEvents = new List<RideEvent>();
    
    private static Database _database;
    private static readonly object padlock = new object();


    public Database()
    {
        UserLoginEvents = new List<LoginEvent>();
    }

    public static Database GetInstance()
    {
        lock (padlock)
        {
            if (_database is null)
            {
                _database = new Database();
            }
    
            return _database;
        }
    }
}