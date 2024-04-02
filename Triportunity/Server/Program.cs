using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server.DataContext;


namespace Server
{
    internal class Program
    {
        private static bool _listenToNewClients = true;
        public static void Main(string[] args)
        {
            // //For bringing up the db just use this method:
            // MemoryDatabase database = MemoryDatabase.GetInstance();

            var localEndPoint = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 4000
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
            receptorSocket.Listen(1000); 
            Console.WriteLine("Waiting for clients...");

            
            Console.ReadKey();
            int users = 1;
            while ( _listenToNewClients)
            {

                Socket transmittorSocket = receptorSocket.Accept();
                int n = users;
                new Thread(() => ManageUser(transmittorSocket, users));
                users++;
            }
        }

        private static void ManageUser(Socket transmittorSocket, int actualUser)
        {
            Console.WriteLine($@"The user {actualUser} is connected");
            byte[] buffer = Array.Empty<byte>();
            int bytesReceived = transmittorSocket.Receive(buffer);

            if (bytesReceived == 0)
            {
                Console.WriteLine("The user has been disconnected");
            }
            else
            {
                string message = Encoding.UTF8.GetString(buffer);
                Console.WriteLine($@"The user {actualUser} : {message}");
            }
            transmittorSocket.Shutdown(SocketShutdown.Both);
            transmittorSocket.Close();
        }
    }
}