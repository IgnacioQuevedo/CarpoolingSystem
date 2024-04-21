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

            Socket newClientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            newClientSocket.Bind(local);
            newClientSocket.Connect(server);
            return newClientSocket;
        }
        public static bool IsSocketConnected(Socket clientSocket)
        {
            try
            {
                string messageReceived = ReceiveMessage(clientSocket);
                Console.WriteLine(messageReceived);
                
                if (messageReceived.Length > 0)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        public static void CloseSocketConnections(Socket clientSocket)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
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
        public static byte[] EncodeMsgIntoBytes(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }
        public static string DecodeMsgFromBytes(byte[] buffer)
        {
            return Encoding.UTF8.GetString(buffer);
        }
        public static void SendMessage(Socket socket, string message)
        {
            Send(socket, BitConverter.GetBytes(message.Length));
            Send(socket, EncodeMsgIntoBytes(message));
        }
        public static string ReceiveMessage(Socket socket)
        {
            byte[] msgLengthBuffer = Receive(socket, ProtocolConstants.DataLengthSize);
            int msgLength = BitConverter.ToInt32(msgLengthBuffer, 0);
            byte[] dataBuffer = Receive(socket, msgLength);
            return DecodeMsgFromBytes(dataBuffer);
        }
        public static void Send(Socket clientSocketServerSide,byte[] buffer)
        {
            int size = buffer.Length;
            int offSet = 0;
            int amountByteSent = 0;

            while (size > 0)
            {
                amountByteSent = clientSocketServerSide.Send(buffer, offSet, size, SocketFlags.None);

                if (amountByteSent == 0) throw new SocketException();

                size = size - amountByteSent;
                offSet = offSet + amountByteSent;
            }
        }
        public static byte[] Receive(Socket clientSocketServerSide,int messageLength)
        {
            byte[] responseBuffer = new byte[messageLength];

            int size = responseBuffer.Length;
            int offSet = 0;
            int amountByteSent = 0;

            while (offSet < size)
            {
                amountByteSent = clientSocketServerSide.Receive(responseBuffer, offSet, size - offSet, SocketFlags.None);

                if (amountByteSent == 0) throw new SocketException();
                
                offSet = offSet + amountByteSent;
            }
            return responseBuffer;
        }
        
    }
}