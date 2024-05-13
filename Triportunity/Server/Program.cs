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
        public static Socket _serverSocket;
        
        private static UserController _userController;
        private static RideController _rideController;
        

        public static void Main(string[] args)
        {
            _serverSocket = NetworkHelper.DeployServerSocket();


            int users = 1;
            while (_listenToNewClients)
            {
                Socket clientSocketServerSide = _serverSocket.Accept();
                _userController = new UserController(clientSocketServerSide);
                _rideController = new RideController(clientSocketServerSide);
                string connectedMsg = "Welcome to Triportunity!! Your user is " + users + "!";
                Console.WriteLine(connectedMsg);

                NetworkHelper.SendMessage(clientSocketServerSide, connectedMsg);
                int actualUser = users;
                new Thread(() => ManageUser(clientSocketServerSide, actualUser)).Start();
                users++;
            }
        }

        private static void ManageUser(Socket clientSocketServerSide, int actualUser)
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
                    string message = NetworkHelper.ReceiveMessage(clientSocketServerSide);
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

                catch (SocketException exceptionNotExpected)
                {
                    Console.WriteLine("Error" + exceptionNotExpected.Message);
                    break;
                }
                catch (Exception exception)
                {
                    throw new Exception(exception.Message);
                }
            }

            NetworkHelper.CloseSocketConnections(clientSocketServerSide);
        }
    }
}