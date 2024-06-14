using Server.Objects.Domain.VehicleModels;

namespace MainServer.Objects.Domain.UserModels;

public class DriverInfo
{
    public double Puntuation { get; set; } = 5.0;
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}