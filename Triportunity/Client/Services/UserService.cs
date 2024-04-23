using System;
using System.Net.Sockets;
using Client.Objects.UserModels;

namespace Client.Services
{
    public class UserService
    {
        public static void RegisterClient(Socket clientSocket, RegisterUserRequest registerUserRequest)
        {
            // Invoke the necessary methods to being possible to communicate with the UserController of backend
            throw new NotImplementedException();
        }

        public static UserClient LoginClient(Socket clientSocket, LoginUserRequest loginUserRequest)
        {
            // Invoke the necessary methods to being possible to communicate with the UserController of backend
            throw new NotImplementedException();
        }

        // public static UserClient UpdateDriver(UpdateUserRequestModel userWithUpdates)
        // {
        //     // Invoke the necessary methods to being possible to communicate with the UserController of backend
        //     throw new NotImplementedException();
        // }
        public static void SetDriverVehicles(Socket clientSocket, string userLoggedUsername)
        {
            throw new NotImplementedException();
        }
    }
}