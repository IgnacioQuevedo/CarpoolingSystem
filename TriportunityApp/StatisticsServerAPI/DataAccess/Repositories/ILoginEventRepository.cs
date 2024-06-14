using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.DataAccess.Repositories;

public interface ILoginEventRepository
{
    public void AddLoginEvent(LoginEvent loginEvent);
}