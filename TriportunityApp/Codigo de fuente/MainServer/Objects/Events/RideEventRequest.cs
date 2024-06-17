namespace MainServer.Objects.Events;

public class RideEventRequest
{
    
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public bool Published { get; set; }
    public List<Guid> Passengers { get; set; }
    public CitiesEnumEventRequest InitialLocation { get; set; }
    public CitiesEnumEventRequest EndingLocation { get; set; }
    public DateTime DepartureTime { get; set; }
    public int AvailableSeats { get; set; }
    public double PricePerPerson { get; set; }
    public bool PetsAllowed { get; set; }
    public Guid VehicleId { get; set; }
}