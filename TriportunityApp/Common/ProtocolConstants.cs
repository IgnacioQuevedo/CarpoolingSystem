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
        public static string Exception = "EXC";
        public static int DataLengthSize = 4;
        public static int MaxUsersInBackLog = 1000;

        #region ImageProtocol

        public const int MaxFileSize = 8; //long
        public const int MaxPartSize = 32768; //32kb

        public static long AmountOfParts(long fileLength)
        {
            long amountOfParts = fileLength / MaxPartSize;
            if (amountOfParts * MaxPartSize != fileLength)
                amountOfParts++;
            return amountOfParts;
        }

        #endregion

    }
}
