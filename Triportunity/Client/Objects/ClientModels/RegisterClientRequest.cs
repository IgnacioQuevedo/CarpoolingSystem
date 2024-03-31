using System.Collections.Generic;
using Client.Objects.VehicleModels;

namespace Client.Objects.ClientModels
{
    public class RegisterClientRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}