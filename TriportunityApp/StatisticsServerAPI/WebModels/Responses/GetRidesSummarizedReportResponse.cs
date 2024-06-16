namespace StatisticsServerAPI.WebModels.Responses;

public class GetRidesSummarizedReportResponse
{
    public IEnumerable<GetRideSummarizedResponse> RidesSummarized { get; set; }
}