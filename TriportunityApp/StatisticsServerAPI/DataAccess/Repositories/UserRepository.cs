using StatisticsServerAPI.DataAccess.MemoryDatabase;
using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    public UserRepository()
    {
        
    }

    public void AddLoginEvent(LoginEvent loginEvent)
    {
        Database.GetInstance().UserLoginEvents.Add(loginEvent);
    }

    public int GetLoginEvents()
    {
        return Database.GetInstance().UserLoginEvents.Count();
    }
}