using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.WebModels.Requests;

public class RideFilterRequest
{
    public Guid? Id { get; set; }

    public Guid? DriverId { get; set; }

    public bool? Published { get; set; }
    
    public CitiesEnumEventRequest? InitialLocation { get; set; }

    public CitiesEnumEventRequest? EndingLocation { get; set; }

    public DateTime? DepartureTime { get; set; }
    public int? AvailableSeats { get; set; }
    public int? PricePerPerson { get; set; }
    public bool? PetsAllowed { get; set; }

    public Guid? VehicleId { get; set; }
}