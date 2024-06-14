using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.DataAccess.Repositories;

public interface IUserRepository
{
    public void AddLoginEvent(LoginEvent loginEvent);
    public int GetLoginEvents();
}