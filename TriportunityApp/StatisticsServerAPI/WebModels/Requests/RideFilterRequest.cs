namespace StatisticsServerAPI.WebModels.Requests;

public class RideFilterRequest
{
    public int? PricePerPerson { get; set; }
    public int? AvailableSeats { get; set; }
    public bool? PetsAllowed { get; set; }
}