using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common;


namespace Server
{
    internal class Program
    {
        private static bool _listenToNewClients = true;
        private static bool _clientWantsToContinueSendingData = true;

        private static Socket _serverSocket;

        public static void Main(string[] args)
        {
            _serverSocket = NetworkHelper.DeployServerSocket();

            int users = 1;
            while (_listenToNewClients)
            {
                Socket clientSocketServerSide = _serverSocket.Accept();
                
                string connectedMsg = "Welcome to the server! You are user " + users + "!";
                
                NetworkHelper.SendMessage(clientSocketServerSide,connectedMsg);
                int actualUser = users;
                new Thread(() => ManageUser(clientSocketServerSide, actualUser)).Start();
                users++;
            }
        }

        private static void ManageUser(Socket clientSocketServerSide, int actualUser)
        {
            Console.WriteLine($@"The user {actualUser} is connected");

            while (_clientWantsToContinueSendingData)
            {
                try
                {
                    byte[] msgLengthBuffer =
                        NetworkHelper.Receive(clientSocketServerSide, ProtocolConstants.DataLengthSize);
                    int msgLength = BitConverter.ToInt32(msgLengthBuffer, 0);

                    byte[] dataBuffer = NetworkHelper.Receive(clientSocketServerSide, msgLength);

                    string message = Encoding.UTF8.GetString(dataBuffer);

                    Console.WriteLine($@"The user {actualUser} : {message}");
                }

                catch (SocketException exceptionNotExpected)
                {
                    Console.WriteLine("Error" + exceptionNotExpected.Message);
                    break;
                }
            }

            NetworkHelper.CloseSocketConnections(clientSocketServerSide);
        }
    }
}