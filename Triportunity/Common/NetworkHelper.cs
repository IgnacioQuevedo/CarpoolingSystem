using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class NetworkHelper
    {
        private Socket _clientSocket;

        public NetworkHelper(Socket clientSocket)
        {
            _clientSocket = clientSocket;
        }

        public Socket ConnectWithServer()
        {
            IPEndPoint local = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 0
            );

            IPEndPoint server = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 5000
            );

            Socket clientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            clientSocket.Bind(local);
            clientSocket.Connect(server);

            return clientSocket;
        }

        public Socket DeployServerSocket(int allowedClientsInBacklog)
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

        public void CloseSocketConnections()
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }

        public byte[] EncodeMsgIntoBytes(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }

        public string DecodeMsgFromBytes(byte[] buffer)
        {
            return Encoding.UTF8.GetString(buffer);
        }


        public void Send(byte[] buffer)
        {
            int size = buffer.Length;
            int offSet = 0;
            int amountByteSend = 0;

            while (size > 0)
            {
                amountByteSend = _clientSocket.Send(buffer, offSet, amountByteSend, SocketFlags.None);

                if (amountByteSend == 0) throw new SocketException();

                size = size - amountByteSend;
                offSet = offSet + amountByteSend;
            }
        }

        public byte[] Receive(int messageLength)
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