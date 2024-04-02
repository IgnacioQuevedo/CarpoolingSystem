using System;
using System.Net;
using System.Net.Sockets;
using Server.DataContext;


namespace Server
{
    internal class Program
    {
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
            
            
                
        }
    }
}