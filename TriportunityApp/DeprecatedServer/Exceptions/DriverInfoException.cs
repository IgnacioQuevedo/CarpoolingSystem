using System;

namespace Server.Exceptions
{
    public class DriverInfoException : Exception
    {
        public DriverInfoException(string message) : base(message)
        {
        }
    }
}