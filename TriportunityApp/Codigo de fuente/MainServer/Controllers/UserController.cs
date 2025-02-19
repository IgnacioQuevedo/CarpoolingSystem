using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MainServer.Objects.Domain.UserModels;
using MainServer.Objects.Domain.VehicleModels;
using MainServer.Objects.Events;
using MainServer.Repositories;
using RabbitMQ.Client;


namespace MainServer.Controllers
{
    public class UserController
    {
        private static UserRepository _userRepository;
        private static CancellationToken _token;

        public UserController(CancellationToken token)
        {
            _userRepository = new UserRepository();
            _token = token;
        }

        public async Task RegisterUserAsync(string[] requestArray, TcpClient _clientServerSide)

        {
            try
            {
                string ci = requestArray[2];
                string username = requestArray[3];
                string password = requestArray[4];
                string repeatedPassword = requestArray[5];

                User userToRegister = new User(ci, username, password, repeatedPassword, null);

                _userRepository.RegisterUser(userToRegister);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.Register + ";" +
                                 userToRegister.Id;
                await NetworkHelper.SendMessageAsync(_clientServerSide, message, _token);
            }
            catch (Exception exception)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exception.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient, _token);
            }
        }

        public async Task LoginUserAsync(string[] requestArray, TcpClient _clientServerSide, IModel channel)
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
                    messageLogin += ";";
                    foreach (var review in userLogged.DriverAspects.Reviews)
                    {
                        messageLogin += review.Id + ":" + review.Punctuation + ":" +
                                        review.Comment + ",";
                    }

                    messageLogin += ";";
                    foreach (var vehicleLogin in userLogged.DriverAspects.Vehicles)
                    {
                        messageLogin += vehicleLogin.Id + ":" + vehicleLogin.VehicleModel + ":" +
                                        vehicleLogin.ImageAllocatedAtServer + ",";
                    }
                }

                await NetworkHelper.SendMessageAsync(_clientServerSide, messageLogin, _token);

                LoginEventRequest userLoginEventRequest = new LoginEventRequest
                {
                    Username = userLogged.Username
                };
                MomHelper.PublishMessage(MomConstraints.exchangeName, MomConstraints.userQueueRoutingKey,
                    userLoginEventRequest, channel);
            }

            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient, _token);
            }
        }

        public async Task CreateDriverAsync(string[] messageArray, TcpClient _clientServerSide)
        {
            try
            {
                Guid userIdToCreate = Guid.Parse(messageArray[2]);

                DriverInfo driverInfo = new DriverInfo();
                _userRepository.RegisterDriver(userIdToCreate, driverInfo);

                string responseMsg = ProtocolConstants.Response + ";" + CommandsConstraints.CreateDriver + ";" +
                                     userIdToCreate;
                await NetworkHelper.SendMessageAsync(_clientServerSide, responseMsg, _token);
            }
            catch (Exception exceptionCaught)
            {
                string excepetionMessageToClient = ProtocolConstants.Exception + ";" +
                                                   CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, excepetionMessageToClient, _token);
            }
        }

        public async Task AddVehicleAsync(string[] messageArray, TcpClient _clientServerSide)
        {
            try
            {
                _token.ThrowIfCancellationRequested();
                Guid userId = Guid.Parse(messageArray[2]);
                string vehicleModel = messageArray[3];
                string path = messageArray[4];

                Vehicle vehicleToAdd = new Vehicle(vehicleModel);
                NetworkHelper.FilePathValidator(path);

                string responseVehicleModelMsg = ProtocolConstants.Response + ";" + CommandsConstraints.AddVehicle + ";"
                                                 + vehicleToAdd.Id;
                Task responseSent = NetworkHelper.SendMessageAsync(_clientServerSide, responseVehicleModelMsg, _token);

                _token.ThrowIfCancellationRequested();
                string imageAllocatedAtServer = await NetworkHelper.ReceiveImageAsync(_clientServerSide, _token);
                vehicleToAdd.ImageAllocatedAtServer = imageAllocatedAtServer;
                _userRepository.AddVehicle(userId, vehicleToAdd);

                await responseSent;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exceptionCaught)
            {
                User userFound = _userRepository.GetUserById(Guid.Parse(messageArray[2]));

                if (userFound.DriverAspects.Vehicles.Count == 0)
                {
                    _userRepository.DeleteDriver(Guid.Parse(messageArray[2]));
                }

                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient, _token);
            }
        }


        public async Task GetUserByIdAsync(string[] messageArray, TcpClient _clientServerSide)
        {
            try
            {
                Guid userId = Guid.Parse(messageArray[2]);
                User userFound = _userRepository.GetUserById(userId);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.GetUserById + ";" + userId +
                                 ";" +
                                 userFound.Ci + ";" + userFound.Username + ";" + userFound.Password + ";";


                if (userFound.DriverAspects != null)
                {
                    message = message + userFound.DriverAspects.Puntuation + ";";
                    if (userFound.DriverAspects.Reviews.Count == 0) message += "#";
                    foreach (var review in userFound.DriverAspects.Reviews)
                    {
                        message += review.Id + ":" + review.Punctuation + ":" + review.Comment + ",";
                    }

                    message += ";";
                    if (userFound.DriverAspects.Vehicles.Count == 0) message += "#";
                    foreach (var vehicle in userFound.DriverAspects.Vehicles)
                    {
                        message += vehicle.Id + ":" + vehicle.VehicleModel + ":" + vehicle.ImageAllocatedAtServer +
                                   ",";
                    }
                }

                await NetworkHelper.SendMessageAsync(_clientServerSide, message, _token);
            }
            catch (Exception exceptionCaught)
            {
                string excepetionMessageToClient = ProtocolConstants.Exception + ";" +
                                                   CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, excepetionMessageToClient, _token);
            }
        }
    }
}