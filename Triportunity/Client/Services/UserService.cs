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

        public static void RegisterClient(Socket socket, RegisterUserRequest registerUserRequest)
        {
            try
            {
                string registerInfo = ProtocolConstants.Request + CommandsConstraints.Register + ";" + registerUserRequest.Username + ";" +
                    registerUserRequest.Password + ";" + registerUserRequest.RepeatedPassword + ";" + registerUserRequest.Ci;

                NetworkHelper.SendMessage(socket, registerInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static UserClient LoginClient(Socket socket, LoginUserRequest loginUserRequest)
        {
            string message = ProtocolConstants.Request + ";" + CommandsConstraints.Login + ";" + loginUserRequest.Username + ";" + loginUserRequest.Password;
            NetworkHelper.SendMessage(socket, message);
            string loginResult = NetworkHelper.ReceiveMessage(socket);

            string[] loginArray = loginResult.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            string username = loginArray[2];
            string password = loginArray[3];
            int puntuation = int.Parse(loginArray[4]);
            List<ReviewClient> reviews = new List<ReviewClient>();
            List<VehicleClient> vehicles = new List<VehicleClient>();
            foreach (var review in loginArray[5].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] reviewArray = review.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                ReviewClient reviewClient = new ReviewClient(Guid.Parse(reviewArray[0]), double.Parse(reviewArray[1]), reviewArray[2]);
                reviews.Add(reviewClient);
            }

            foreach (var vehicle in loginArray[6].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] vehicleArray = vehicle.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                VehicleClient vehicleClient = new VehicleClient(Guid.Parse(vehicleArray[0]), vehicleArray[1]);
                vehicles.Add(vehicleClient);
            }

            DriverInfoClient driverInfo = new DriverInfoClient(puntuation, reviews, vehicles);

            UserClient user = new UserClient(Id,username, password, driverInfo);

            return user;
        }

        public static void SetDriverVehicles(Socket socket, string username)
        {
            Console.WriteLine("Please enter the path of the vehicle image");
            string path = Console.ReadLine();

            string message = ProtocolConstants.Request + ";" + CommandsConstraints.SetVehicle + ";" + username;
            NetworkHelper.SendMessage(socket, message);

            NetworkHelper.SendImageFromClient(socket, path);

        }

        public static UserClient FindUserById(Socket socket, Guid id)
        {
            string message = ProtocolConstants.Request + ";" + CommandsConstraints.FindUserById + ";" + id.ToString();
            NetworkHelper.SendMessage(socket, message);

            string userResult = NetworkHelper.ReceiveMessage(socket);

            string[] userArray = userResult.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            string username = userArray[2];
            string password = userArray[3];
            int puntuation = int.Parse(userArray[4]);
            List<ReviewClient> reviews = new List<ReviewClient>();
            List<VehicleClient> vehicles = new List<VehicleClient>();
            foreach (var review in userArray[5].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] reviewArray = review.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                ReviewClient reviewClient = new ReviewClient(Guid.Parse(reviewArray[0]), double.Parse(reviewArray[1]), reviewArray[2]);
                reviews.Add(reviewClient);
            }
            foreach (var vehicle in userArray[6].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] vehicleArray = vehicle.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                VehicleClient vehicleClient = new VehicleClient(Guid.Parse(vehicleArray[0]), vehicleArray[1]);
                vehicles.Add(vehicleClient);
            }

            DriverInfoClient driverInfo = new DriverInfoClient(puntuation, reviews, vehicles);

            UserClient user = new UserClient(username, password, driverInfo);

            return user;
        }
    }
}