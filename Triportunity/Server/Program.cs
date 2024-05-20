using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Server.Controllers;


namespace Server
{
    internal class Program
    {
        private static bool _listenToNewClients = true;
        private static TcpListener _serverListener;

        private static UserController _userController;
        private static RideController _rideController;

        private static ConcurrentBag<TcpClient> _clientsConnected = new ConcurrentBag<TcpClient>();
        private static CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private static CancellationToken _token = _cancellationToken.Token;


        public static async Task Main(string[] args)
        {
            try
            {
                _serverListener = NetworkHelper.DeployServerListener();
                _serverListener.Start(ProtocolConstants.MaxUsersInBackLog);
                _ = WaitUntilAdminShutdownServer();
                int users = 1;

                while (!_token.IsCancellationRequested)
                {
                    TcpClient clientServerSide =
                        await _serverListener.AcceptTcpClientAsync(_token);

                    _clientsConnected.Add(clientServerSide);

                    _userController = new UserController(clientServerSide, _token);
                    _rideController = new RideController(clientServerSide, _token);

                    int actualUser = users;
                    if (_token.IsCancellationRequested) break;
                    _ = ManageUserAsync(clientServerSide, actualUser);

                    users++;
                }
            }

            catch (ObjectDisposedException)
            {
                Console.WriteLine("Server shutdown");
            }

            catch (Exception exception)
            {
                Console.WriteLine($"Error not expected : {exception.Message} - shutting down server");
                while (!_clientsConnected.IsEmpty)
                {
                    _clientsConnected.TryTake(out TcpClient client);
                    NetworkHelper.CloseTcpConnection(client);
                }

                _serverListener.Stop();
            }
        }

        private static async Task WaitUntilAdminShutdownServer()
        {
            Console.WriteLine("Do you want to shutdown the server? - Press x to shutdown");

            await Task.Run(() =>
            {
                while (_token.IsCancellationRequested == false)
                {
                    string response = Console.ReadLine()?.ToUpper();

                    if (!string.IsNullOrEmpty(response) && response.Equals("X"))
                    {
                        _cancellationToken.Cancel();
                        ShutdownOperations();
                        _serverListener.Stop();
                    }
                }
            });
        }

        private static void ShutdownOperations()
        {
            if (_token.IsCancellationRequested)
            {
                Console.WriteLine("Do you want to close all connections instantly? - Press X");
                Console.WriteLine("Do you want to close all connections after they end their petition? - Press Y");
                string response = Console.ReadLine().ToUpper();

                if (response.Equals("X"))
                {
                    //Close all connections right now
                    while (!_clientsConnected.IsEmpty)
                    {
                        if (_clientsConnected.TryTake(out TcpClient client))
                        {
                            NetworkHelper.CloseTcpConnection(client);
                        }
                    }
                }

                else if (response.Equals("Y"))
                {
                    Console.WriteLine("Waiting for clients to end their petitions");
                    //Close all connections after they end their petition
                    while (_clientsConnected.Count > 0)
                    {
                    }
                }
                else
                {
                    Console.WriteLine("Invalid option, insert X or Z");
                    ShutdownOperations();
                }
            }
        }

        private static async Task ManageUserAsync(TcpClient clientServerSide, int actualUser)
        {
            string connectedMsg = "Welcome to Triportunity!! Your user is " + actualUser + "!";
            Console.WriteLine(connectedMsg);
            await NetworkHelper.SendMessageAsync(clientServerSide, connectedMsg);
            bool clientWantsToContinueSendingData = true;

            while (clientWantsToContinueSendingData && !_token.IsCancellationRequested)
            {
                try
                {
                    string message = await NetworkHelper.ReceiveMessageAsync(clientServerSide);
                    Console.WriteLine($@"The user {actualUser} : {message}");

                    string[] messageArray = message.Split(new string[] { ";" }, StringSplitOptions.None);
                    int command = int.Parse(messageArray[1]);

                    switch (command)
                    {
                        case CommandsConstraints.Login:
                            await _userController.LoginUserAsync(messageArray);
                            break;

                        case CommandsConstraints.Register:

                            await _userController.RegisterUserAsync(messageArray);
                            break;

                        case CommandsConstraints.CreateDriver:

                            await _userController.CreateDriverAsync(messageArray);
                            break;
                        case CommandsConstraints.GetUserById:

                            await _userController.GetUserByIdAsync(messageArray);
                            break;

                        case CommandsConstraints.AddVehicle:

                            await _userController.AddVehicleAsync(messageArray);
                            break;

                        case CommandsConstraints.CreateRide:

                            await _rideController.CreateRide(messageArray);

                            break;

                        case CommandsConstraints.JoinRide:

                            await _rideController.JoinRide(messageArray);

                            break;

                        case CommandsConstraints.EditRide:
                            await _rideController.EditRide(messageArray);

                            break;

                        case CommandsConstraints.DeleteRide:
                            await _rideController.DeleteRide(messageArray);

                            break;

                        case CommandsConstraints.QuitRide:
                            await _rideController.QuitRide(messageArray);

                            break;

                        case CommandsConstraints.FilterRidesByPrice:
                            await _rideController.FilterRidesByPrice(messageArray);
                            break;
                        case CommandsConstraints.GetAllRides:
                            await _rideController.GetAllRides();
                            break;

                        case CommandsConstraints.GetCarImage:
                            await _rideController.GetCarImage(messageArray);
                            break;

                        case CommandsConstraints.GetDriverReviews:
                            await _rideController.GetDriverReviews(messageArray);
                            break;

                        case CommandsConstraints.DisableRide:
                            await _rideController.DisableRide(messageArray);
                            break;

                        case CommandsConstraints.GetRideById:
                            await _rideController.GetRideById(messageArray);
                            break;

                        case CommandsConstraints.AddReview:
                            await _rideController.AddReview(messageArray);
                            break;

                        case CommandsConstraints.GetRidesByUser:
                            await _rideController.GetRidesByUser(messageArray);
                            break;

                        case CommandsConstraints.CloseApp:
                            clientWantsToContinueSendingData = false;
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }


                catch (Exception exceptionNotExpected)
                {
                    Console.WriteLine("Error: " + exceptionNotExpected.Message);
                    break;
                }
            }

            foreach (TcpClient client in _clientsConnected)
            {
                if (client == clientServerSide)
                {
                    _clientsConnected.TryTake(out _); // Vaciar la referencia del cliente
                    break;
                }
            }

        }
    }
}