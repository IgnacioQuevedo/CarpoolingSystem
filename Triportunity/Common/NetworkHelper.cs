using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
            return transmitterSocket;
        }

        public static Socket DeployServerSocket(int allowedClientsInBacklog)
        {
            var localEndPoint = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 5000
            );

            var serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            serverSocket.Bind(localEndPoint);
            serverSocket.Listen(allowedClientsInBacklog);
            Console.WriteLine("Waiting for clients...");
            return serverSocket;
        }

        public static void CloseSocketConnections(Socket clientSocket)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        public static byte[] EncodeMsg(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }
    }
}