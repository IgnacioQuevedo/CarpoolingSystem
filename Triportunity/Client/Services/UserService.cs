using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using Client.Objects.ReviewModels;
using Client.Objects.UserModels;
using Client.Objects.VehicleModels;
using Common;

namespace Client.Services
{
    public class UserService
    {
        private static Socket _clientSocket;

        public UserService(Socket socketClient)
        {
            _clientSocket = socketClient;
        }

        public void RegisterClient(Socket socket, RegisterUserRequest registerUserRequest)
        {
            try
            {
                string registerInfo = ProtocolConstants.Request + ";" + CommandsConstraints.Register + ";" +
                                      registerUserRequest.Ci + ";" +
                                      registerUserRequest.Username + ";" +
                                      registerUserRequest.Password + ";" + registerUserRequest.RepeatedPassword;

                NetworkHelper.SendMessage(socket, registerInfo);

                string serverResponse = NetworkHelper.ReceiveMessage(socket);

                string[] responseArray =
                    serverResponse.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                if (responseArray[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(responseArray[2]);
                }

                if (responseArray[0] == ProtocolConstants.Response)
                {
                    Console.WriteLine("User registered successfully");
                }
                else
                {
                    throw new Exception("Error registering user");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public UserClient LoginClient(Socket socket, LoginUserRequest loginUserRequest)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.Login + ";" +
                                 loginUserRequest.Username + ";" + loginUserRequest.Password;
                NetworkHelper.SendMessage(socket, message);

                UserClient resultUser = null;

                string loginResult = NetworkHelper.ReceiveMessage(socket);

                string[] loginArray = loginResult.Split(new string[] { ";" }, StringSplitOptions.None);


                if (loginArray[0] != ProtocolConstants.Exception)
                {
                    Guid id = Guid.Parse(loginArray[2]);
                    string ci = loginArray[3];
                    string username = loginArray[4];
                    string password = loginArray[5];
                    DriverInfoClient driverInfo = null;

                    if (loginArray.Length > 6)
                    {
                        List<ReviewClient> reviews = new List<ReviewClient>();
                        List<VehicleClient> vehicles = new List<VehicleClient>();

                        if (loginArray[6] != "")
                        {
                            foreach (var review in loginArray[6]
                                         .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                string[] reviewArray =
                                    review.Split(new string[] { ":" }, StringSplitOptions.None);
                                ReviewClient reviewClient = new ReviewClient(Guid.Parse(reviewArray[0]),
                                    double.Parse(reviewArray[1]), reviewArray[2]);
                                reviews.Add(reviewClient);
                            }
                        }

                        foreach (var vehicle in loginArray[7]
                                     .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string[] vehicleArray =
                                vehicle.Split(new string[] { ":" }, StringSplitOptions.None);
                            VehicleClient vehicleClient = new VehicleClient(Guid.Parse(vehicleArray[0]),
                                vehicleArray[1],
                                vehicleArray[2]);
                            vehicles.Add(vehicleClient);
                        }

                        driverInfo = new DriverInfoClient(reviews, vehicles);
                    }

                    resultUser = new UserClient(id, ci, username, password, driverInfo);
                }
                else
                {
                    throw new Exception(loginArray[2]);
                }

                return resultUser;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void CreateDriver(Socket socket, Guid userId, string carModel, string path)
        {
            try
            {
                string messageToSend = ProtocolConstants.Request + ";" + CommandsConstraints.CreateDriver + ";" +
                                       userId;

                NetworkHelper.SendMessage(socket, messageToSend);

                string messageReceived = NetworkHelper.ReceiveMessage(_clientSocket);

                string[] messageArray =
                    messageReceived.Split(new string[] { ";" }, StringSplitOptions.None);

                if (messageArray[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(messageArray[2]);
                }

                AddVehicle(socket, userId, carModel, path);
                Console.WriteLine("You are now a driver");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void AddVehicle(Socket clientSocket, Guid userRegisteredId, string carModel, string path)
        {
            try
            {
                
                SetDriverVehicles(clientSocket, userRegisteredId, carModel, path);

                string messageArray = NetworkHelper.ReceiveMessage(clientSocket);

                string[] vehicleInfoArray =
                    messageArray.Split(new string[] { ";" }, StringSplitOptions.None);

                if (vehicleInfoArray[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(vehicleInfoArray[2]);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetDriverVehicles(Socket socket, Guid userId, string carModel, string path)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.AddVehicle + ";" + userId + ";" +
                                 carModel;
                NetworkHelper.SendMessage(socket, message);
                
                NetworkHelper.SendImage(socket, path);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public UserClient GetUserById(Socket clientSocket, Guid userId)
        {
            try
            {
                double generalPunctuation = -1;

                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetUserById + ";" + userId;
                NetworkHelper.SendMessage(clientSocket, message);

                string messageArray = NetworkHelper.ReceiveMessage(clientSocket);

                string[] userArray = messageArray.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                if (userArray[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(userArray[2]);
                }

                Guid id = Guid.Parse(userArray[2]);
                string ci = userArray[3];
                string username = userArray[4];
                string password = userArray[5];

                DriverInfoClient driverInfo = null;

                if (userArray.Length > 6)
                {
                    generalPunctuation = double.Parse(userArray[6]);
                    List<ReviewClient> reviews = new List<ReviewClient>();
                    List<VehicleClient> vehicles = new List<VehicleClient>();

                    if (!userArray[7].Equals("#"))
                    {
                        string[] reviewsArray =
                            userArray[7].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var review in reviewsArray)
                        {
                            string[] reviewArray =
                                review.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                            Guid reviewId = Guid.Parse(reviewArray[0]);
                            int punctuation = int.Parse(reviewArray[1]);
                            string comment = reviewArray[2];

                            ReviewClient reviewClient = new ReviewClient(reviewId, punctuation, comment);
                            reviews.Add(reviewClient);
                        }
                    }

                    if (!userArray[8].Equals("#"))
                    {
                        string[] vehiclesArray =
                            userArray[8].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var vehicle in vehiclesArray)
                        {
                            string[] vehicleArray =
                                vehicle.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                            Guid vehicleId = Guid.Parse(vehicleArray[0]);
                            string vehicleModel = vehicleArray[1];
                            string imageAllocatedAtAServer = vehicleArray[2];
                            VehicleClient vehicleClient =
                                new VehicleClient(vehicleId, vehicleModel, imageAllocatedAtAServer);
                            vehicles.Add(vehicleClient);
                        }
                    }

                    driverInfo = new DriverInfoClient(reviews, vehicles);
                }

                UserClient user = new UserClient(id, ci, username, password, driverInfo);

                if (generalPunctuation != -1)
                {
                    user.DriverAspects.Punctuation = generalPunctuation;
                }

                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ICollection<VehicleClient> GetVehiclesByUserId(Guid userLoggedId)
        {
            try
            {
                UserClient user = GetUserById(_clientSocket, userLoggedId);
                if (user.DriverAspects != null) return user.DriverAspects.Vehicles;
                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void CloseApp(Socket clientSocket)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.CloseApp;
                NetworkHelper.SendMessage(clientSocket, message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}