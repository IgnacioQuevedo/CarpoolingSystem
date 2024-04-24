using System;
using System.Net.Sockets;
using Client.Objects.UserModels;
using Common;


namespace Client.Services
{
    public class UserService
    {
            

        public static void RegisterClient(Socket socket, RegisterUserRequest registerUserRequest)
        {
            try
            {
                string registerInfo = ProtocolConstants.Request + CommandsConstraints.Register + ";" + registerUserRequest.Username + ";" + registerUserRequest.Password + ";" + registerUserRequest.RepeatedPassword + ";" + registerUserRequest.Ci;

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

            UserClient user = new UserClient();

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

        public static UserClient FindUserByUsername(string username)
        {
            return UserController.FindUserByUsername(username);
        }
    }
}