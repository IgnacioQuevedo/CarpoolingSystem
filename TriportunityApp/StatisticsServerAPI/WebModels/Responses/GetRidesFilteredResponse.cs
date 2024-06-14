using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.WebModels.Responses;

public class GetRidesFilteredResponse
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public bool Published { get; set; }
    public List<Guid> Passengers { get; set; }
    public CitiesEnumEventResponse InitialLocation { get; set; }
    public CitiesEnumEventResponse EndingLocation { get; set; }
    public DateTime DepartureTime { get; set; }
    public int AvailableSeats { get; set; }
    public double PricePerPerson { get; set; }
    public bool PetsAllowed { get; set; }
    public Guid VehicleId { get; set; }
}