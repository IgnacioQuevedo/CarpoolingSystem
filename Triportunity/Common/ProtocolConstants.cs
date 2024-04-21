using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ProtocolConstants
    {
        public static string Request = "REQ";
        public static string Response = "RES";
        public static int DirectionLength = 3;
        public static int CommandLength = 2;
        public static int DataLengthSize = 4;
        public static int MaxUsersInBackLog = 1000;
        
    }
}
