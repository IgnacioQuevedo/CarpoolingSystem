namespace StatisticsServerAPI.MqDomain;

public class RideEvent
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public bool Published { get; set; }
    public List<Guid> Passengers { get; set; }
    public CitiesEnumEvent InitialLocation { get; set; }
    public CitiesEnumEvent EndingLocation { get; set; }
    public DateTime DepartureTime { get; set; }
    public int AvailableSeats { get; set; }
    public double PricePerPerson { get; set; }
    public bool PetsAllowed { get; set; }
    public Guid VehicleId { get; set; }
    
}