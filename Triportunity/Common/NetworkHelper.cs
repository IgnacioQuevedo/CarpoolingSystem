using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    public static class NetworkHelper
    {
        private static Socket _clientSocket;

        public static Socket ConnectWithServer()
        {
            IPEndPoint local = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 0
            );

            IPEndPoint server = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 5000
            );

            Socket newClientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            newClientSocket.Bind(local);
            newClientSocket.Connect(server);
            _clientSocket = newClientSocket;
            
            return newClientSocket;
        }

        public static Socket DeployServerSocket()
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
            serverSocket.Listen(ProtocolConstants.MaxUsersInBackLog);
            Console.WriteLine("Waiting for clients...");
            return serverSocket;
        }

        public static void CloseSocketConnections()
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }

        public static byte[] EncodeMsgIntoBytes(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }

        public static string DecodeMsgFromBytes(byte[] buffer)
        {
            return Encoding.UTF8.GetString(buffer);
        }

        public static void Send(byte[] buffer)
        {
            int size = buffer.Length;
            int offSet = 0;
            int amountByteSent = 0;

            while (size > 0)
            {
                amountByteSent = _clientSocket.Send(buffer, offSet, size, SocketFlags.None);

                if (amountByteSent == 0) throw new SocketException();

                size = size - amountByteSent;
                offSet = offSet + amountByteSent;
            }
        }

        public static byte[] Receive(int messageLength)
        {
            byte[] buffer = new byte[messageLength];

            int size = buffer.Length;
            int offSet = 0;
            int amountByteSent = 0;

            while (size > 0)
            {
                amountByteSent = _clientSocket.Receive(buffer, offSet, size, SocketFlags.None);

                if (amountByteSent == 0) throw new SocketException();

                size = size - amountByteSent;
                offSet = offSet + amountByteSent;
            }

            return buffer;
        }
    }
}