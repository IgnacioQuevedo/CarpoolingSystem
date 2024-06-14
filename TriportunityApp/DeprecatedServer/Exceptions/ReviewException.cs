using System;

namespace Server.Exceptions
{
    public class ReviewException : Exception
    {
        public ReviewException(string message) : base(message) { }
    }
}
