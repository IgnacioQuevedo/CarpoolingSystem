using System;

namespace Server.Exceptions
{
    public class ClientException : Exception
    {
        public ClientException(string message) : base(message)
        {
        }
    }
}