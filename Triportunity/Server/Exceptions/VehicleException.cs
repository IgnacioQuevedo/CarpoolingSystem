using System;


namespace Server.Exceptions
{
    public class VehicleException : Exception
    {
        public VehicleException(string message) : base(message)
        {
        }
    }
}
