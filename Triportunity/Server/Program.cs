using System;
using System.Net.Sockets;
using System.Threading;
using Common;
using Server.Controllers;
using Server.Repositories;


namespace Server
{
    internal class Program
    {
        private static bool _listenToNewClients = true;
        public static TcpListener _serverListener;
        
        private static UserController _userController;
        private static RideController _rideController;
        

        public static void Main(string[] args)
        {
            _serverListener = NetworkHelper.DeployServerListener();

            int users = 1;
            while (_listenToNewClients)
            {
                _serverListener.Start(ProtocolConstants.MaxUsersInBackLog);
                TcpClient clientServerSide = _serverListener.AcceptTcpClient();
                _userController = new UserController(clientServerSide);
                _rideController = new RideController(clientServerSide);
                string connectedMsg = "Welcome to Triportunity!! Your user is " + users + "!";
                Console.WriteLine(connectedMsg);

                NetworkHelper.SendMessage(clientServerSide, connectedMsg);
                int actualUser = users;
                new Thread(() => ManageUser(clientServerSide, actualUser)).Start();
                users++;
            }
        }

        private static void ManageUser(TcpClient clientServerSide, int actualUser)
        {
            bool _clientWantsToContinueSendingData = true;
            string direction = "";
            int command = 0;
            string username = "";
            string password = "";

            while (_clientWantsToContinueSendingData)
            {
                try
                {
                    string message = NetworkHelper.ReceiveMessage(clientServerSide);
                    Console.WriteLine($@"The user {actualUser} : {message}");

                    string[] messageArray = message.Split(new string[] { ";" }, StringSplitOptions.None);
                    command = int.Parse(messageArray[1]);

                    switch (command)
                    {
                        case CommandsConstraints.Login:
                            _userController.LoginUser(messageArray);
                            break;

                        case CommandsConstraints.Register:

                            _userController.RegisterUser(messageArray);
                            break;

                        case CommandsConstraints.CreateDriver:

                            _userController.CreateDriver(messageArray);
                            break;
                        case CommandsConstraints.GetUserById:

                            _userController.GetUserById(messageArray);
                            break;

                        case CommandsConstraints.AddVehicle:

                            _userController.AddVehicle(messageArray);
                            break;

                        case CommandsConstraints.CreateRide:

                            _rideController.CreateRide(messageArray);

                            break;

                        case CommandsConstraints.JoinRide:

                            _rideController.JoinRide(messageArray);

                            break;

                        case CommandsConstraints.EditRide:
                            _rideController.EditRide(messageArray);

                            break;

                        case CommandsConstraints.DeleteRide:
                            _rideController.DeleteRide(messageArray);

                            break;

                        case CommandsConstraints.QuitRide:
                            _rideController.QuitRide(messageArray);

                            break;

                        case CommandsConstraints.FilterRidesByPrice:
                            _rideController.FilterRidesByPrice(messageArray);
                            break;
                        case CommandsConstraints.GetAllRides:
                            _rideController.GetAllRides();
                            break;

                        case CommandsConstraints.GetCarImage:
                            _rideController.GetCarImage(messageArray);
                            break;

                        case CommandsConstraints.GetDriverReviews:
                            _rideController.GetDriverReviews(messageArray);
                            break;

                        case CommandsConstraints.DisableRide:
                            _rideController.DisableRide(messageArray);
                            break;

                        case CommandsConstraints.GetRideById:
                            _rideController.GetRideById(messageArray);
                            break;

                        case CommandsConstraints.AddReview:
                            _rideController.AddReview(messageArray);
                            break;
                     
                          case CommandsConstraints.GetRidesByUser:
                            _rideController.GetRidesByUser(messageArray);
                            break;
                        
                        case CommandsConstraints.CloseApp:
                            _clientWantsToContinueSendingData = false;
                            break;
                    }
                }

                catch (Exception exceptionNotExpected)  
                {
                    Console.WriteLine("Error" + exceptionNotExpected.Message);
                    break;
                }
            }

            NetworkHelper.CloseTcpConnections(clientServerSide);
            _serverListener.Stop();
        }
    }
}