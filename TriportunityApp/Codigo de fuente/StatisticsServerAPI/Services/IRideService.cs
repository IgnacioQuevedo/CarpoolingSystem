using StatisticsServerAPI.Domain;
using StatisticsServerAPI.MqDomain;
using StatisticsServerAPI.WebModels.Requests;
using StatisticsServerAPI.WebModels.Responses;

namespace StatisticsServerAPI.Services;

public interface IRideService
{
    public IEnumerable<GetRideFilteredResponse> GenerateRidesFiltered(RideFilter filters);
    public RidesSummarizedReport GetRidesSummarizedReportById(Guid id);
    public bool AskForCompleteness(Guid id);
    public CreateRidesSummarizedReportResponse CreateRidesSummarizedReport(int amountOfNextRidesToSummarize);
    
}