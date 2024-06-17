using StatisticsServerAPI.DataAccess.Repositories;

namespace StatisticsServerAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public int GetLoginEvents()
    {
        return _userRepository.GetLoginEvents();
    }
}