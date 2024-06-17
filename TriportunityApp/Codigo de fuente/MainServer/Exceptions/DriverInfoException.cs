using System;

namespace MainServer.Exceptions
{
    public class DriverInfoException : Exception
    {
        public DriverInfoException(string message) : base(message)
        {
        }
    }
}