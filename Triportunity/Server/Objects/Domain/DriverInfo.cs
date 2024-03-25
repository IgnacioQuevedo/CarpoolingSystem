using System.Collections;
using System.Collections.Generic;

namespace Server.Objects.Domain
{
    public class DriverInfo
    {
        public int Ci { get; set; }
        public int Puntuation { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public IEnumerable<Vehicle> Vehicles { get; set; }
        
        
        
    }
}