using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Common;
using Server.Controllers;
using Server.Objects.Domain;
using Server.Objects.Domain.Enums;
using Server.Objects.Domain.UserModels;
using Server.Objects.Domain.VehicleModels;
using Server.Objects.DTOs.RideModelDtos;
using Server.Repositories;


namespace Server
{
    internal class Program
    {
        private static bool _listenToNewClients = true;
        private static bool _clientWantsToContinueSendingData = true;


        private static UserController _userController;
        private static UserRepository _userRepository;
        private static RideRepository _rideRepository;
        public static Socket _serverSocket;
        public static void Main(string[] args)
        {
            _serverSocket = NetworkHelper.DeployServerSocket();
            
            _userRepository = new UserRepository();
            _rideRepository = new RideRepository();


            int users = 1;
            while (_listenToNewClients)
            {
                Socket clientSocketServerSide = _serverSocket.Accept();
                _userController = new UserController(clientSocketServerSide);
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

                    string[] messageArray = message.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    command = int.Parse(messageArray[1]);

                    Guid userId;
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

                        case CommandsConstraints.GetCarImage:

                            userId = Guid.Parse(messageArray[2]);
                            Guid vehicleId = Guid.Parse(messageArray[3]);

                            Vehicle vehicleFound = _userRepository.GetVehicleById(userId, vehicleId);

                            NetworkHelper.SendImage(clientSocketServerSide, vehicleFound.ImageAllocatedAtAServer);

                            break;

                        case CommandsConstraints.AddVehicle:
                            
                            _userController.AddVehicle(messageArray);
                            break;

                        case CommandsConstraints.CreateRide:

                            string[] rideInfoArray = message.Split(new string[] { ";" },
                                StringSplitOptions.RemoveEmptyEntries);

                            List<Guid> passengers = new List<Guid>();
                            for (int i = 8; i < rideInfoArray.Length; i++)
                            {
                                Guid passengerId = Guid.Parse(rideInfoArray[i]);
                                passengers.Add(passengerId);
                            }

                            Guid id = Guid.Parse(rideInfoArray[2]);
                            CitiesEnum initialLocation = (CitiesEnum)int.Parse(rideInfoArray[3]);
                            CitiesEnum endingLocation = (CitiesEnum)int.Parse(rideInfoArray[4]);
                            DateTime departureTime = DateTime.Parse(rideInfoArray[5]);
                            int availableSeats = int.Parse(rideInfoArray[6]);
                            double price = double.Parse(rideInfoArray[7]);
                            bool pets = bool.Parse(rideInfoArray[8]);
                            string path = rideInfoArray[9];

                            // Ride ride = new Ride(id, initialLocation, endingLocation, departureTime, availableSeats
                            //     , price, pets, path, passengers);

                            // RideRepository.CreateRide(ride);

                            break;

                        case CommandsConstraints.JoinRide:
                            string joinRideInfo = NetworkHelper.ReceiveMessage(_serverSocket);

                            string[] rideArray = joinRideInfo.Split(new string[] { ";" },
                                StringSplitOptions.RemoveEmptyEntries);

                            userId = Guid.Parse(rideArray[2]);
                            Guid rideId = Guid.Parse(rideArray[3]);

                            RideRepository.JoinRide(userId, rideId);

                            break;

                        case CommandsConstraints.EditRide:
                            string editedRide = NetworkHelper.ReceiveMessage(_serverSocket);

                            string[] editInfo = editedRide.Split(new string[] { ";" },
                                StringSplitOptions.RemoveEmptyEntries);

                            Guid originalId = Guid.Parse(editInfo[2]);
                            CitiesEnum initialLocationEdit = (CitiesEnum)int.Parse(editInfo[3]);
                            CitiesEnum finalLocation = (CitiesEnum)int.Parse(editInfo[4]);
                            DateTime deapartureTimeEdit = DateTime.Parse(editInfo[5]);
                            int priceEdit = int.Parse(editInfo[6]);
                            bool petsAllowed = bool.Parse(editInfo[7]);
                            string photoPath = editInfo[8];
                            bool published = bool.Parse(editInfo[9]);

                            // ModifyRideRequestDto udaptedRide = new ModifyRideRequestDto(originalId, initialLocationEdit,
                            //     finalLocation, deapartureTimeEdit, priceEdit, petsAllowed, photoPath, published);
                            //
                            // _rideRepository.UpdateRide(udaptedRide);

                            break;

                        case CommandsConstraints.DeleteRide:
                            Guid rideToDeleteId = Guid.Parse(NetworkHelper.ReceiveMessage(_serverSocket));

                            _rideRepository.DeleteRide(rideToDeleteId);

                            break;

                        case CommandsConstraints.QuitRide:
                            string quitRideInfo = NetworkHelper.ReceiveMessage(_serverSocket);

                            string[] quitRideArray = quitRideInfo.Split(new string[] { ";" },
                                StringSplitOptions.RemoveEmptyEntries);

                            Guid userQuitId = Guid.Parse(quitRideArray[2]);
                            Guid rideQuitId = Guid.Parse(quitRideArray[3]);

                            _rideRepository.QuitRide(userQuitId, rideQuitId);

                            break;
                    }
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