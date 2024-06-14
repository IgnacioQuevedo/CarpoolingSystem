using Common;
using MainServer.Controllers;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace MainServer
{
    internal class Server
    {
        private static bool _listenToNewClients = true;
        private static TcpListener _serverListener;

        private static UserController _userController;
        private static RideController _rideController;

        private static object _lock = new object();

        private static ConcurrentBag<TcpClient> _clientsConnected = new ConcurrentBag<TcpClient>();
        private static CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private static CancellationToken _token = _cancellationToken.Token;

        public static async Task LaunchServerAsync()
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



                    int actualUser = users;
                    if (_token.IsCancellationRequested) break;
                    _ = ManageUserAsync(clientServerSide, actualUser);

                    users++;
                }
            }

            catch (OperationCanceledException)
            {
                ShutdownOperations();
                _serverListener.Stop();
                Console.WriteLine("Server shutdown");
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

                    //While por despertares espurios
                    lock (_lock)
                    {
                        while (_clientsConnected.Count > 0)
                        {
                            Monitor.Wait(_lock);
                        }
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

            _userController = new UserController(_token);
            _rideController = new RideController(_token);

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
                            await _userController.LoginUserAsync(messageArray, clientServerSide);
                            break;

                        case CommandsConstraints.Register:

                            await _userController.RegisterUserAsync(messageArray, clientServerSide);
                            break;

                        case CommandsConstraints.CreateDriver:

                            await _userController.CreateDriverAsync(messageArray, clientServerSide);
                            break;
                        case CommandsConstraints.GetUserById:

                            await _userController.GetUserByIdAsync(messageArray, clientServerSide);
                            break;

                        case CommandsConstraints.AddVehicle:

                            await _userController.AddVehicleAsync(messageArray, clientServerSide);
                            break;

                        case CommandsConstraints.CreateRide:

                            await _rideController.CreateRide(messageArray, clientServerSide);

                            break;

                        case CommandsConstraints.JoinRide:

                            await _rideController.JoinRide(messageArray, clientServerSide);

                            break;

                        case CommandsConstraints.EditRide:
                            await _rideController.EditRide(messageArray, clientServerSide);

                            break;

                        case CommandsConstraints.DeleteRide:
                            await _rideController.DeleteRide(messageArray, clientServerSide);

                            break;

                        case CommandsConstraints.QuitRide:
                            await _rideController.QuitRide(messageArray, clientServerSide);

                            break;

                        case CommandsConstraints.FilterRidesByPrice:
                            await _rideController.FilterRidesByPrice(messageArray, clientServerSide);
                            break;
                        case CommandsConstraints.GetAllRides:
                            await _rideController.GetAllRides(clientServerSide);
                            break;

                        case CommandsConstraints.GetCarImage:
                            await _rideController.GetCarImage(messageArray, clientServerSide);
                            break;

                        case CommandsConstraints.GetDriverReviews:
                            await _rideController.GetDriverReviews(messageArray, clientServerSide);
                            break;

                        case CommandsConstraints.DisableRide:
                            await _rideController.DisableRide(messageArray, clientServerSide);
                            break;

                        case CommandsConstraints.GetRideById:
                            await _rideController.GetRideById(messageArray, clientServerSide);
                            break;

                        case CommandsConstraints.AddReview:
                            await _rideController.AddReview(messageArray, clientServerSide);
                            break;

                        case CommandsConstraints.GetRidesByUser:
                            await _rideController.GetRidesByUser(messageArray, clientServerSide);
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

            TcpClient someClient = new TcpClient();
            bool clientNotFound = true;
            while (clientNotFound)
            {

                _clientsConnected.TryTake(out someClient);

                if (someClient != clientServerSide)
                {
                    _clientsConnected.Add(someClient);
                }
                else
                {
                    lock (_lock)
                    {
                        if (_clientsConnected.IsEmpty) { Monitor.PulseAll(_lock); }
                    }
                    clientNotFound = false;
                }
            }

        }
    }
}
