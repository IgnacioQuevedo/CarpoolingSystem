namespace StatisticsServerAPI.Domain;

public class RidesSummarizedReport
{

    public Guid Id { get; set; } = Guid.NewGuid();
    public List<RidesSummarized> RidesSummarized { get; set; } = new List<RidesSummarized>();
    public int AmountOfNextRidesToSummarize { get; set; }
}