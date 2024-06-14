using StatisticsServerAPI.MqDomain;
using StatisticsServerAPI.WebModels.Requests;
using StatisticsServerAPI.WebModels.Responses;

namespace StatisticsServerAPI.Services;

public interface IRideService
{
    public IEnumerable<GetRidesFilteredResponse> GetRidesFiltered(RideFilterDto filters);
}