using StatisticsServerAPI.DataAccess.MemoryDatabase;
using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.DataAccess.Repositories;

public class LoginEventRepository : ILoginEventRepository
{
    public LoginEventRepository()
    {
        
    }

    public void AddLoginEvent(LoginEvent loginEvent)
    {
        Database.GetInstance().UserLoginEvents.Add(loginEvent);
    }
}