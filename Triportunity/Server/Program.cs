using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Server
{
    internal class Program
    {
        private static bool _listenToNewClients = true;
        private static bool _clientWantsToContinueSendingData = true;
        private static int _maxUsersInBackLog = 1000;

        public static void Main(string[] args)
        {
            // //For bringing up the db just use this method:
            // MemoryDatabase database = MemoryDatabase.GetInstance();

            var localEndPoint = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 5000
            );

            var receptorSocket = new Socket(
                // We declare that the socket will use IP
                AddressFamily.InterNetwork,
                // For being possible to use transport protocol we must declare it as a stream socket
                SocketType.Stream,
                ProtocolType.Tcp
            );

            //This will connect the socket with the IP and Port define above
            receptorSocket.Bind(localEndPoint);
            //This is how much users could be in a waiting state before entering the system
            receptorSocket.Listen(_maxUsersInBackLog);
            Console.WriteLine("Waiting for clients...");
            int users = 1;

            while (_listenToNewClients)
            {
                //Accepts the new connection request of a transmittor socket
                Socket transmitterSocket = receptorSocket.Accept();
                Console.WriteLine(transmitterSocket);
                int actualUser = users;
                new Thread(() => ManageUser(transmitterSocket, actualUser)).Start();
                users++;
            }
        }

        private static void ManageUser(Socket transmittorSocket, int actualUser)
        {
            Console.WriteLine($@"The user {actualUser} is connected");

            while (_clientWantsToContinueSendingData)
            {
                var buffer = new byte[10];
                try
                {
                    // Waits for the client to send the data (It gets in suspended state)
                    int bytesReceived = transmittorSocket.Receive(buffer);

                    if (bytesReceived == 0)
                    {
                        Console.WriteLine("The user has been disconnected");
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer);
                    Console.WriteLine($@"The user {actualUser} : {message}");
                }

                catch (SocketException e)
                {
                    Console.WriteLine("Error" + e.Message);
                    break;
                }
            }

            transmittorSocket.Shutdown(SocketShutdown.Both);
            transmittorSocket.Close();
        }
    }
}