using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Common;
using Server.Objects.Domain;
using Server.Objects.Domain.ClientModels;
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
            string direction = "";
            int command = 0;

            while (_clientWantsToContinueSendingData)
            {
                try
                {
                    string message = NetworkHelper.ReceiveMessage(clientSocketServerSide);
                    Console.WriteLine($@"The user {actualUser} : {message}");

                    string[] messageArray = message.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                    direction = messageArray[0];

                    if ((messageArray[0].Equals(ProtocolConstants.Response) || messageArray[0].Equals(ProtocolConstants.Response)) && int.TryParse(messageArray[1], out command))
                    {

                    }

                    switch (command)
                    {
                        case CommandsConstraints.Login:

                            string[] loginInfoArray = message.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                            UserRepository.Login(loginInfoArray[2], loginInfoArray[3]);

                            break;

                        case CommandsConstraints.Register:


                            string registerMessage = NetworkHelper.ReceiveMessage(clientSocketServerSide);

                            string[] userInfoArray = registerMessage.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                            string username = userInfoArray[2];

                            string password = userInfoArray[3];

                            string repeatedPassword = userInfoArray[4];

                            string ci = userInfoArray[5];

                            double punctuation = int.Parse(userInfoArray[6]);

                            ICollection<Review> reviews = new List<Review>();

                            DriverInfo driverAspects = null;

                            User user = new User(userInfoArray[2], userInfoArray[3], userInfoArray[4], driverAspects);

                            UserRepository.RegisterClient(user);

                            break;

                        case CommandsConstraints.SetVehicle:

                            string[] userToEditArray = message.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                            username = userToEditArray[2];

                            string filePath = NetworkHelper.ReceiveImageToServer(clientSocketServerSide);

                            User userFound = UserRepository.FindClientViaUsername(username);

                            userFound.DriverAspects = new DriverInfo();

                            Vehicle vehicle = new Vehicle(filePath);

                            userFound.DriverAspects.Vehicles.Add(vehicle);

                            break;
                        case CommandsConstraints.CreateRide:

                            string[] rideInfoArray = message.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

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

                            Ride ride = new Ride(id, initialLocation, endingLocation, departureTime, availableSeats
                                , price, pets, path, passengers);

                            RideRepository.CreateRide(ride);

                            break;

                        case CommandsConstraints.JoinRide:
                            string joinRideInfo = NetworkHelper.ReceiveMessage(_serverSocket);

                            string[] rideArray = joinRideInfo.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                            Guid userId = Guid.Parse(rideArray[2]);
                            Guid rideId = Guid.Parse(rideArray[3]);

                            RideRepository.JoinRide(userId, rideId);

                            break;

                        case CommandsConstraints.EditRides:
                            string editedRide = NetworkHelper.ReceiveMessage(_serverSocket);

                            string[] editInfo = editedRide.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                            Guid originalId = Guid.Parse(editInfo[2]);
                            CitiesEnum initialLocationEdit = (CitiesEnum)int.Parse(editInfo[3]);
                            CitiesEnum finalLocation = (CitiesEnum)int.Parse(editInfo[4]);
                            DateTime deapartureTimeEdit = DateTime.Parse(editInfo[5]);
                            int priceEdit = int.Parse(editInfo[6]);
                            bool petsAllowed = bool.Parse(editInfo[7]);
                            string photoPath = editInfo[8];
                            bool published = bool.Parse(editInfo[9]);

                            ModifyRideRequestDto udaptedRide = new ModifyRideRequestDto(originalId, initialLocationEdit, finalLocation, deapartureTimeEdit, priceEdit, petsAllowed, photoPath, published);

                            RideRepository.UpdateRide(udaptedRide);

                            break;

                        case CommandsConstraints.DeleteRide:
                            Guid rideToDeleteId = Guid.Parse(NetworkHelper.ReceiveMessage(_serverSocket));

                            RideRepository.DeleteRide(rideToDeleteId);

                            break;

                        case CommandsConstraints.QuitRide:
                            string quitRideInfo = NetworkHelper.ReceiveMessage(_serverSocket);

                            string[] quitRideArray = quitRideInfo.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                            Guid userQuitId = Guid.Parse(quitRideArray[2]);
                            Guid rideQuitId = Guid.Parse(quitRideArray[3]);

                            RideRepository.QuitRide(userQuitId, rideQuitId);

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