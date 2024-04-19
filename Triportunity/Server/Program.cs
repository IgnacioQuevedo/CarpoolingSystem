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
        private static NetworkHelper _networkHelper;

        public Program(NetworkHelper networkHelper)
        {
            _networkHelper = networkHelper;
            _serverSocket = _networkHelper.DeployServerSocket(ProtocolConstants.MaxUsersInBackLog);
        }
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
                try
                {
                    byte[]  msgLengthBuffer = _networkHelper.Receive(ProtocolConstants.DataLengthSize);
                    int msgLength = BitConverter.ToInt32(msgLengthBuffer, 0);
                    
                    byte[] dataBuffer = _networkHelper.Receive(msgLength);
                    
                    string message = Encoding.UTF8.GetString(dataBuffer);
                    
                    Console.WriteLine($@"The user {actualUser} : {message}");
                }

                catch (SocketException exceptionNotExpected)
                {
                    Console.WriteLine("Error" + exceptionNotExpected.Message);
                    break;
                }
            }

            _networkHelper.CloseSocketConnections();
        }
        
    }
}