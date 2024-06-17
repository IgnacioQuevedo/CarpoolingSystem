using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Client.Objects.ReviewModels;
using Client.Objects.UserModels;
using Client.Objects.VehicleModels;
using Common;

namespace Client.Services
{
    public class UserService
    {
        public UserService()
        {
        }

        public async Task RegisterClientAsync(TcpClient client, RegisterUserRequest registerUserRequest, CancellationToken token)
        {
            try
            {
                string registerInfo = ProtocolConstants.Request + ";" + CommandsConstraints.Register + ";" +
                                      registerUserRequest.Ci + ";" +
                                      registerUserRequest.Username + ";" +
                                      registerUserRequest.Password + ";" + registerUserRequest.RepeatedPassword;

                await NetworkHelper.SendMessageAsync(client, registerInfo, token);

                string serverResponse = await NetworkHelper.ReceiveMessageAsync(client, token);

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
                throw new Exception(e.Message, e);
            }
        }

        public async Task<UserClient> LoginClientAsync(TcpClient client, LoginUserRequest loginUserRequest, CancellationToken token)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.Login + ";" +
                                 loginUserRequest.Username + ";" + loginUserRequest.Password;

                UserClient resultUser = null;
                await NetworkHelper.SendMessageAsync(client, message, token);

                string loginResult = await NetworkHelper.ReceiveMessageAsync(client, token);

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
                throw new Exception(e.Message, e);
            }
        }

        public async Task CreateDriverAsync(TcpClient client, Guid userId, string carModel, string path, CancellationToken token)
        {
            try
            {
                string messageToSend = ProtocolConstants.Request + ";" + CommandsConstraints.CreateDriver + ";" +
                                       userId;

                await NetworkHelper.SendMessageAsync(client, messageToSend, token);

                string messageReceived = await NetworkHelper.ReceiveMessageAsync(client, token);

                string[] messageArray =
                    messageReceived.Split(new string[] { ";" }, StringSplitOptions.None);

                if (messageArray[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(messageArray[2]);
                }

                await AddVehicleAsync(client, userId, carModel, path, token);
                Console.WriteLine("You are now a driver");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public async Task AddVehicleAsync(TcpClient client, Guid userId, string carModel, string path, CancellationToken token)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.AddVehicle + ";" + userId + ";" +
                                 carModel + ";" + path;
                await NetworkHelper.SendMessageAsync(client, message, token);

                string messageArray = await NetworkHelper.ReceiveMessageAsync(client, token);
                string[] vehicleInfoArray =
                    messageArray.Split(new string[] { ";" }, StringSplitOptions.None);

                if (vehicleInfoArray[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(vehicleInfoArray[2]);
                }

                await NetworkHelper.SendImageAsync(client, path, token);
            }
            catch (Exception exceptionCaught)
            {
                NetworkHelper.CheckIfExceptionIsOperationCanceled(exceptionCaught);
                throw new Exception(exceptionCaught.Message, exceptionCaught);
            }
        }

        public async Task<UserClient> GetUserByIdAsync(TcpClient client, Guid userId, CancellationToken token)
        {
            try
            {
                double generalPunctuation = -1;

                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetUserById + ";" + userId;
                await NetworkHelper.SendMessageAsync(client, message, token);

                string messageArray = await NetworkHelper.ReceiveMessageAsync(client, token);

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
                NetworkHelper.CheckIfExceptionIsOperationCanceled(e);
                throw new Exception(e.Message, e);
            }
        }

        public async Task<ICollection<VehicleClient>> GetVehiclesByUserIdAsync(TcpClient client, Guid userLoggedId, CancellationToken token)
        {
            try
            {
                UserClient user = await GetUserByIdAsync(client, userLoggedId, token);
                if (user.DriverAspects != null) return user.DriverAspects.Vehicles;
                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public async Task CloseAppAsync(TcpClient client, CancellationToken token)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.CloseApp;
                await NetworkHelper.SendMessageAsync(client, message, token);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

    }
}