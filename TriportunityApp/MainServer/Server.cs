using Common;
using System.Collections.Concurrent;
using System.Net.Sockets;
using MainServer.Services;
using MainServer.Controllers;

namespace MainServer
{
    public class Server
    {
        private static bool _listenToNewClients = true;
        private static TcpListener _serverListener;

        private static UserController _userController;
        //private static AdminClient _adminClient; 

        private static object _lock = new object();

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

                //_adminClient = new AdminClient("https://localhost:5001"); 

                int users = 1;

                while (!_token.IsCancellationRequested)
                {
                    TcpClient clientServerSide = await _serverListener.AcceptTcpClientAsync(_token);

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
                    Console.WriteLine("Invalid option, insert X or Y");
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

                        //case CommandsConstraints.CreateRide:
                        //    var createRideRequest = new TripRequest { TripId = int.Parse(messageArray[2]), Destination = messageArray[3], Price = double.Parse(messageArray[4]) };
                        //    var createRideResponse = await _adminClient.AddTripAsync(createRideRequest);
                        //    await NetworkHelper.SendMessageAsync(clientServerSide, createRideResponse.Status);
                        //    break;

                        case CommandsConstraints.JoinRide:
                            // Similar logic for other CRUD operations
                            break;

                        //case CommandsConstraints.EditRide:
                        //    var editRideRequest = new TripRequest { TripId = int.Parse(messageArray[2]), Destination = messageArray[3], Price = double.Parse(messageArray[4]) };
                        //    var editRideResponse = await _adminClient.UpdateTripAsync(editRideRequest);
                        //    await NetworkHelper.SendMessageAsync(clientServerSide, editRideResponse.Status);
                        //    break;

                        //case CommandsConstraints.DeleteRide:
                        //    var deleteRideRequest = new TripRequest { TripId = int.Parse(messageArray[2]) };
                        //    var deleteRideResponse = await _adminClient.DeleteTripAsync(deleteRideRequest);
                        //    await NetworkHelper.SendMessageAsync(clientServerSide, deleteRideResponse.Status);
                        //    break;

                        // Add cases for other commands...

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
