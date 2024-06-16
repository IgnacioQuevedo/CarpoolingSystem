using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.Domain;

public class RideFilter
{
    public Guid? Id { get; set; }
    
    public Guid? DriverId { get; set; }
    public bool? Published { get; set; }
    
    public CitiesEnumEvent? InitialLocation { get; set; }
    
    public CitiesEnumEvent? EndingLocation { get; set; }
    
    public DateTime? DepartureTime { get; set; }
    public int? AvailableSeats { get; set; }
    public int? PricePerPerson { get; set; }
    public bool? PetsAllowed { get; set; }
    
    public Guid? VehicleId { get; set; }
}