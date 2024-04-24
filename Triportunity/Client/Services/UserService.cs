using System;
using System.Collections.Generic;
using System.Net.Sockets;
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

        public static void RegisterClient(Socket socket, RegisterUserRequest registerUserRequest)
        {
            try
            {
                string registerInfo = ProtocolConstants.Request + CommandsConstraints.Register + ";" +
                                      registerUserRequest.Username + ";" +
                                      registerUserRequest.Password + ";" + registerUserRequest.RepeatedPassword + ";" +
                                      registerUserRequest.Ci;

                NetworkHelper.SendMessage(socket, registerInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static UserClient LoginClient(Socket socket, LoginUserRequest loginUserRequest)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.Login + ";" +
                                 loginUserRequest.Username + ";" + loginUserRequest.Password;
                NetworkHelper.SendMessage(socket, message);
                string loginResult = NetworkHelper.ReceiveMessage(socket);

                string[] loginArray = loginResult.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                Guid id = Guid.Parse(loginArray[2]);
                string ci = loginArray[3];
                string username = loginArray[4];
                string password = loginArray[5];
                int puntuation = int.Parse(loginArray[6]);
                List<ReviewClient> reviews = new List<ReviewClient>();
                List<VehicleClient> vehicles = new List<VehicleClient>();
                foreach (var review in loginArray[7].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] reviewArray = review.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    ReviewClient reviewClient = new ReviewClient(Guid.Parse(reviewArray[0]),
                        double.Parse(reviewArray[1]), reviewArray[2]);
                    reviews.Add(reviewClient);
                }

                foreach (var vehicle in loginArray[8]
                             .Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] vehicleArray = vehicle.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    VehicleClient vehicleClient = new VehicleClient(Guid.Parse(vehicleArray[0]), vehicleArray[1]);
                    vehicles.Add(vehicleClient);
                }

                DriverInfoClient driverInfo = new DriverInfoClient(puntuation, reviews, vehicles);

                UserClient user = new UserClient(id, ci, username, password, driverInfo);

                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void SetDriverVehicles(Socket socket, string username)
        {
            Console.WriteLine("Please enter the path of the vehicle image");
            string path = Console.ReadLine();

            string message = ProtocolConstants.Request + ";" + CommandsConstraints.CreateDriver + ";" + username;
            NetworkHelper.SendMessage(socket, message);

            NetworkHelper.SendImageFromClient(socket, path);
        }

        public static UserClient FindUserById(Socket socket, Guid id)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.FindUserById + ";" +
                                 id.ToString();
                NetworkHelper.SendMessage(socket, message);

                string userResult = NetworkHelper.ReceiveMessage(socket);

                string[] userArray = userResult.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                string ci = userArray[2];
                string username = userArray[3];
                string password = userArray[4];
                int puntuation = int.Parse(userArray[5]);
                List<ReviewClient> reviews = new List<ReviewClient>();
                List<VehicleClient> vehicles = new List<VehicleClient>();
                foreach (var review in userArray[6].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] reviewArray = review.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    ReviewClient reviewClient = new ReviewClient(Guid.Parse(reviewArray[0]),
                        double.Parse(reviewArray[1]), reviewArray[2]);
                    reviews.Add(reviewClient);
                }

                foreach (var vehicle in userArray[7].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] vehicleArray = vehicle.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    VehicleClient vehicleClient = new VehicleClient(Guid.Parse(vehicleArray[0]), vehicleArray[1]);
                    vehicles.Add(vehicleClient);
                }

                DriverInfoClient driverInfo = new DriverInfoClient(puntuation, reviews, vehicles);

                UserClient user = new UserClient(id, ci, username, password, driverInfo);

                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}