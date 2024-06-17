using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer.Exceptions
{
    public class RideException : Exception
    {
        public RideException(string message) : base(message)
        {

        }
    }
}
