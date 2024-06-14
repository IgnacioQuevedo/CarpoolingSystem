using System;

namespace MainServer.Exceptions
{
    public class ReviewException : Exception
    {
        public ReviewException(string message) : base(message) { }
    }
}
