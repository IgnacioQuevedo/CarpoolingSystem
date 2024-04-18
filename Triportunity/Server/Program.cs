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
        private static int _maxUsersInBackLog = 1000;
        
        private static Socket _serverSocket = NetworkHelper.DeployServerSocket(_maxUsersInBackLog);
        public static void Main(string[] args)
        {
            int users = 1;
            while (_listenToNewClients)
            {
                Socket clientConnectedSocket = _serverSocket.Accept();
                int actualUser = users;
                new Thread(() => ManageUser(clientConnectedSocket, actualUser)).Start();
                users++;
            }
        }

        private static void ManageUser(Socket clientConnectedSocket, int actualUser)
        {
            Console.WriteLine($@"The user {actualUser} is connected");

            while (_clientWantsToContinueSendingData)
            {
                var buffer = new byte[10];
                try
                {
                    int bytesReceived = clientConnectedSocket.Receive(buffer);

                    if (bytesReceived == 0)
                    {
                        Console.WriteLine("The user has been disconnected");
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer);
                    Console.WriteLine($@"The user {actualUser} : {message}");
                }

                catch (SocketException exceptionNotExpected)
                {
                    Console.WriteLine("Error" + exceptionNotExpected.Message);
                    break;
                }
            }

            NetworkHelper.CloseSocketConnections();
        }
    }
}