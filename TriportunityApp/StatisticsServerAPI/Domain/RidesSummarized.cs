using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.Domain;

public class RidesSummarized
{
    public bool Published { get; set; }
    public List<Guid> Passengers { get; set; }
    public CitiesEnumEvent InitialLocation { get; set; }
    public DateTime DepartureTime { get; set; }
    public int AvailableSeats { get; set; }
    public double PricePerPerson { get; set; }
}