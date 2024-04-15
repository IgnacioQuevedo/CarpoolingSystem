using System.Net;
using System.Net.Sockets;

namespace Common
{
    public static class NetworkHelper
    {
        public static Socket ConnectWithServer()
        {
            IPEndPoint local = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 0
            );
            IPEndPoint server = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 5000
            );

            Socket transmitterSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            transmitterSocket.Bind(local);
            transmitterSocket.Connect(server);
            return transmitterSocket;
        }

        public static byte[] EncodeMsg(string message)
        {
            
        }

        
    }
}