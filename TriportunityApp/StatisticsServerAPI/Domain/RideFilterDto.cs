namespace StatisticsServerAPI.MqDomain;

public class RideFilterDto
{
    public int? PricePerPerson { get; set; }
    public int? AvailableSeats { get; set; }
    public bool? PetsAllowed { get; set; }
}