namespace StatisticsServerAPI.WebModels.Responses;

public class GetRideSummarizedResponse
{
    public bool Published { get; set; }
    public List<Guid> Passengers { get; set; }
    public CitiesEnumEventResponse InitialLocation { get; set; }
    public DateTime DepartureTime { get; set; }
    public int AvailableSeats { get; set; }
    public double PricePerPerson { get; set; }
}