using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Client.Objects.ReviewModels;
using Client.Objects.UserModels;
using Client.Objects.VehicleModels;
using Common;
using Server.Exceptions;
using Server.Objects.Domain;
using Server.Objects.Domain.UserModels;
using Server.Objects.Domain.VehicleModels;
using Server.Objects.DTOs.UserModelDtos;
using Server.Repositories;


namespace Server.Controllers
{
    public class UserController
    {
        private static UserRepository _userRepository = new UserRepository();
        private static RideRepository _rideRepository = new RideRepository();
        private static Socket _serverSocket;

        public UserController(Socket socket)
        {
            _serverSocket = socket;
        }

        #region COMPLETADOS

        public void RegisterUser(string[] requestArray)
        {
            try
            {
                string ci = requestArray[2];
                string username = requestArray[3];
                string password = requestArray[4];
                string repeatedPassword = requestArray[5];

                User userToRegister = new User(ci, username, password, repeatedPassword, null);
                _userRepository.RegisterUser(userToRegister);
            }
            catch (UserException exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public void LoginUser(string[] requestArray)
        {
            try
            {
                string username = requestArray[2];
                string password = requestArray[3];

                User userLogged = _userRepository.Login(username, password);

                string messageLogin = ProtocolConstants.Response + ";" + CommandsConstraints.Login + ";" +
                                      userLogged.Id + ";" +
                                      userLogged.Ci + ";" + userLogged.Username + ";" + userLogged.Password;

                if (userLogged.DriverAspects != null)
                {
                    foreach (var review in userLogged.DriverAspects.Reviews)
                    {
                        messageLogin += review.Id + ":" + review.Punctuation + ":" +
                                        review.Comment + ",";
                    }

                    messageLogin += ";";
                    foreach (var vehicleLogin in userLogged.DriverAspects.Vehicles)
                    {
                        messageLogin += vehicleLogin.Id + ":" + vehicleLogin.VehicleModel + ":" +
                                        vehicleLogin.ImageAllocatedAtAServer + ",";
                    }
                }

                NetworkHelper.SendMessage(_serverSocket, messageLogin);
            }

            catch (UserException exceptionCaught)
            {
                string excpetionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_serverSocket, excpetionMessageToClient);
            }
        }

        
        public void CreateDriver(string[] messageArray)
        {
            try
            {
                Guid userId = Guid.Parse(messageArray[2]);
                DriverInfo driverInfo = new DriverInfo(new List<Vehicle>());
                _userRepository.RegisterDriver(userId, driverInfo);

                string responseMsg = ProtocolConstants.Response + ";" + CommandsConstraints.CreateDriver + ";" + userId;
                NetworkHelper.SendMessage(_serverSocket, responseMsg);
            }
            catch (UserException exceptionCaught)
            {
                throw new Exception(exceptionCaught.Message);
            }
        }
        
        public void AddVehicle(string[] messageArray)
        {
            try
            { ;
                Guid userId = Guid.Parse(messageArray[2]);
                string vehicleModel = messageArray[3];
                
                string imageAllocatedAtAServer = NetworkHelper.ReceiveImage(_serverSocket);
                Vehicle vehicleToAdd = new Vehicle(vehicleModel, imageAllocatedAtAServer);
                
                _userRepository.AddVehicle(userId, vehicleToAdd);
                
                string responseMsg = ProtocolConstants.Response + ";" + CommandsConstraints.AddVehicle + ";" 
                                     + vehicleToAdd.Id + ";" + vehicleToAdd.VehicleModel + ";" + vehicleToAdd.ImageAllocatedAtAServer;
                
                NetworkHelper.SendMessage(_serverSocket, responseMsg);
            }
            catch (UserException exceptionCaught)
            {
                throw new Exception(exceptionCaught.Message);
            }
        }

      

        public void GetUserById(string[] messageArray)
        {
            Guid userId = Guid.Parse(messageArray[2]);
            User userFound = _userRepository.GetUserById(userId);

            string message = ProtocolConstants.Response + ";" + CommandsConstraints.GetUserById + ";" + userId + ";" +
                             userFound.Ci + ";" + userFound.Username + ";" + userFound.Password + ";";

            if (userFound.DriverAspects != null)
            {
                message = message + userFound.DriverAspects.Puntuation + ";";
                if(userFound.DriverAspects.Reviews.Count == 0) message += "#";
                foreach (var review in userFound.DriverAspects.Reviews)
                {
                    message += review.Id + ":" + review.Punctuation + ":" + review.Comment + ",";
                }

                message += ";";
                if(userFound.DriverAspects.Vehicles.Count == 0) message += "#";
                foreach (var vehicle in userFound.DriverAspects.Vehicles)
                {
                    message += vehicle.Id + ":" + vehicle.VehicleModel + ":" + vehicle.ImageAllocatedAtAServer + ",";
                }
            
            }
            NetworkHelper.SendMessage(_serverSocket, message);
        }

        #endregion
        
        // public static void AddReview(Guid driverId, ReviewClient reviewToAdd)
        // {
        //     try
        //     {
        //         _userRepository.AddReview(driverId, new Review(reviewToAdd.Punctuation, reviewToAdd.Comment));
        //     }
        //     catch (UserException exceptionCaught)
        //     {
        //         throw new Exception(exceptionCaught.Message);
        //     }
        // }
    }
}