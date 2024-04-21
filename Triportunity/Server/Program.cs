using System;
using System.Net.Sockets;
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

                string connectedMsg = "Welcome to Triportunity!! Your user is " + users + "!";

                NetworkHelper.SendMessage(clientSocketServerSide, connectedMsg);
                int actualUser = users;
                new Thread(() => ManageUser(clientSocketServerSide, actualUser)).Start();
                users++;
            }
        }
        private static void ManageUser(Socket clientSocketServerSide, int actualUser)
        {
            while (_clientWantsToContinueSendingData)
            {
                try
                {
                    string message = NetworkHelper.ReceiveMessage(clientSocketServerSide);
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