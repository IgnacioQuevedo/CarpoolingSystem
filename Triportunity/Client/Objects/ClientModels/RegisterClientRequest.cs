using System.Collections.Generic;
using ClientUI.Objects.VehicleModels;

namespace ClientUI.Objects.ClientModels
{
    public class RegisterClientRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}