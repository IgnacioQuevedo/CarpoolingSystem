using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MainServer.Exceptions;
using MainServer.Objects.Domain;
using MainServer.Objects.Domain.Enums;
using MainServer.Objects.Domain.VehicleModels;
using MainServer.Objects.Events;
using MainServer.Repositories;
using RabbitMQ.Client;
using Server.Repositories;

namespace MainServer.Controllers
{
    public class RideController
    {
        private static RideRepository _rideRepository = new RideRepository();
        private static UserRepository _userRepository = new UserRepository();
        private static CancellationToken _token = new CancellationToken();
        
        public RideController(CancellationToken token)
        {
            _token = token;
        }

        public async Task CreateRide(string[] messageArray, TcpClient _clientServerSide, IModel channel)
        {
            try
            {
                List<Guid> passengers = new List<Guid>();

                Guid driverId = Guid.Parse(messageArray[2]);

                CitiesEnum initialLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), messageArray[3]);
                CitiesEnum endingLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), messageArray[4]);
                DateTime departureTime = DateTime.Parse(messageArray[5]);
                int availableSeats = int.Parse(messageArray[6]);
                double price = double.Parse(messageArray[7]);
                bool pets = bool.Parse(messageArray[8]);
                Guid vehicleId = Guid.Parse(messageArray[9]);

                Ride ride = new Ride(driverId, initialLocation, endingLocation, departureTime, availableSeats,
                    price, pets, vehicleId);

                ride.Id = Guid.NewGuid();

                _rideRepository.CreateRide(ride);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.CreateRide + ";" + ride.Id;

                await NetworkHelper.SendMessageAsync(_clientServerSide, message);
                
                
                RideEventRequest rideEventRequest = new RideEventRequest
                {
                    Id = ride.Id,
                    DriverId = ride.DriverId,
                    Published = ride.Published,
                    Passengers = ride.Passengers,
                    InitialLocation = (CitiesEnumEventRequest)ride.InitialLocation,
                    EndingLocation = (CitiesEnumEventRequest)ride.EndingLocation,
                    DepartureTime = ride.DepartureTime,
                    AvailableSeats = ride.AvailableSeats,
                    PricePerPerson = ride.PricePerPerson,
                    PetsAllowed = ride.PetsAllowed,
                    VehicleId = ride.VehicleId
                };
                MomHelper.PublishMessage(MomConstraints.exchangeName, MomConstraints.rideQueueRoutingKey, rideEventRequest,channel);
                
                
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient);
            }
        }

        public async Task GetRidesByUser(string[] messageArray, TcpClient _clientServerSide)
        {
            try
            {
                Guid userId = Guid.Parse(messageArray[2]);
                ICollection<Ride> rides = _rideRepository.GetRidesByUser(userId);

                string response = ProtocolConstants.Response + ";" + CommandsConstraints.GetRidesByUser + ";";

                foreach (var ride in rides)
                {
                    response += ride.Id + ":" + ride.DriverId + ":";

                    foreach (var passenger in ride.Passengers)
                    {
                        response += passenger + ",";
                    }

                    response += ":" + ride.InitialLocation +
                                ":" + ride.EndingLocation + ":" + ride.DepartureTime + ":" + ride.AvailableSeats + ":" +
                                ride.PricePerPerson + ":" + ride.PetsAllowed + ":" + ride.VehicleId + "@";
                }

                await NetworkHelper.SendMessageAsync(_clientServerSide, response);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient);
            }
        }

        public async Task JoinRide(string[] messageArray, TcpClient _clientServerSide)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                Guid userId = Guid.Parse(messageArray[3]);

                _rideRepository.JoinRide(userId, rideId);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.JoinRide;

                await NetworkHelper.SendMessageAsync(_clientServerSide, message);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient);
            }
        }

        public async Task EditRide(string[] messageArray, TcpClient _clientServerSide)
        {
            try
            {

                Guid rideId = Guid.Parse(messageArray[2]);
      
                CitiesEnum initialLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), messageArray[3]);
                CitiesEnum endingLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), messageArray[4]);
                DateTime departureTime = DateTime.Parse(messageArray[5]);
                int availableSeats = int.Parse(messageArray[6]);
                double price = double.Parse(messageArray[7]);
                bool pets = bool.Parse(messageArray[8]);
                Guid vehicleId = Guid.Parse(messageArray[9]);
                Guid driverId = Guid.Parse(messageArray[10]);

                Ride ride = new Ride(driverId, initialLocation, endingLocation, departureTime, availableSeats,
                    price, pets, vehicleId);

                ride.Id = rideId;

                _rideRepository.UpdateRide(ride);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.EditRide + ";" + ride.Id;

                await NetworkHelper.SendMessageAsync(_clientServerSide, message);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient);
            }
        }

        public async Task DeleteRide(string[] messageArray,TcpClient _clientServerSide)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                _rideRepository.DeleteRide(rideId);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.DeleteRide;

                await NetworkHelper.SendMessageAsync(_clientServerSide, message);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient);
            }
        }

        public async Task QuitRide(string[] messageArray,TcpClient _clientServerSide)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                Guid userId = Guid.Parse(messageArray[3]);

                _rideRepository.QuitRide(rideId, userId);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.QuitRide;
                await NetworkHelper.SendMessageAsync(_clientServerSide, message);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient);
            }
        }

        public async Task GetAllRides(TcpClient _clientServerSide)
        {
            try
            {
                ICollection<Ride> rides = _rideRepository.GetRides();

                string response = ProtocolConstants.Response + ";" + CommandsConstraints.GetAllRides + ";";

                foreach (var ride in rides)
                {
                    response += ride.Id + ":" + ride.DriverId + ":";

                    foreach (var passenger in ride.Passengers)
                    {
                        response += passenger + ",";
                    }

                    response += ":" + ride.InitialLocation +
                                ":" + ride.EndingLocation + ":" + ride.DepartureTime + ":" + ride.AvailableSeats + ":" +
                                ride.PricePerPerson + ":" + ride.PetsAllowed + ":"
                                + ride.VehicleId + "@";
                }

                await NetworkHelper.SendMessageAsync(_clientServerSide, response);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient);
            }
        }

        public async Task GetCarImage(string[] messageArray,TcpClient _clientServerSide)
        {
            try
            {
                if(_token.IsCancellationRequested) return;
                Guid userId = Guid.Parse(messageArray[2]);
                Guid rideId = Guid.Parse(messageArray[3]);
                Ride rideToFound = _rideRepository.GetRideById(rideId);
                Guid vehicleId = rideToFound.VehicleId;
                Vehicle vehicle = _userRepository.GetVehicleById(userId, vehicleId);

                await NetworkHelper.SendImageAsync(_clientServerSide, vehicle.ImageAllocatedAtServer,_token);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient);
            }
        }

        public async Task DisableRide(string[] messageArray,TcpClient _clientServerSide)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                _rideRepository.DisablePublishedRide(rideId);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.DisableRide;
                await NetworkHelper.SendMessageAsync(_clientServerSide, message);
            }
            catch (Exception e)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + e.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient);
            }
        }

        public async Task FilterRidesByPrice(string[] messageArray, TcpClient _clientServerSide)
        {
            try
            {
                double minPrice = double.Parse(messageArray[2]);
                double maxPrice = double.Parse(messageArray[3]);

                if (maxPrice < 0 || minPrice < 0 || maxPrice <= minPrice)
                {
                    throw new RideException("Max price cannot be equal or lower than min price");
                }

                ICollection<Ride> rides = _rideRepository.FilterByPrice(minPrice, maxPrice);

                string response = ProtocolConstants.Response + ";" + CommandsConstraints.FilterRidesByPrice + ";";


                foreach (var ride in rides)
                {
                    response += ride.Id + ":" + ride.DriverId + ":";

                    foreach (var passenger in ride.Passengers)
                    {
                        response += passenger + ",";
                    }

                    response += ":" + ride.InitialLocation +
                                ":" + ride.EndingLocation + ":" + ride.DepartureTime + ":" + ride.AvailableSeats + ":" +
                                ride.PricePerPerson + ":" + ride.PetsAllowed + ":"
                                + ride.VehicleId + "@";
                }

                await NetworkHelper.SendMessageAsync(_clientServerSide, response);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient);
            }
        }

        public async Task GetDriverReviews(string[] messageArray, TcpClient _clientServerSide)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                Ride ride = _rideRepository.GetRideById(rideId);

                string response = ProtocolConstants.Response + ";" + CommandsConstraints.GetDriverReviews + ";";

                ICollection<Review> reviews = _rideRepository.GetDriverReviews(rideId);

                foreach (var review in reviews)
                {
                    response += review.Id + ":" + review.Punctuation + ":" + review.Comment + ",";
                }

                await NetworkHelper.SendMessageAsync(_clientServerSide, response);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient);
            }
        }


        public async Task GetRideById(string[] messageArray, TcpClient _clientServerSide)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                Ride ride = _rideRepository.GetRideById(rideId);

                string response = ProtocolConstants.Response + ";" + CommandsConstraints.GetRideById + ";";

                response += ride.Id + ":" + ride.DriverId + ":";

                foreach (var passenger in ride.Passengers)
                {
                    response += passenger + ",";
                }

                response += ":" + ride.InitialLocation +
                            ":" + ride.EndingLocation + ":" + ride.DepartureTime + ":" + ride.AvailableSeats + ":" +
                            ride.PricePerPerson + ":" + ride.PetsAllowed + ":"
                            + ride.VehicleId;

                await NetworkHelper.SendMessageAsync(_clientServerSide, response);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient);
            }
        }

        public async Task AddReview(string[] messageArray, TcpClient _clientServerSide)
        {
            try
            {
                Guid actualUser = Guid.Parse(messageArray[2]);
                Guid rideId = Guid.Parse(messageArray[3]);
                double punctuation = double.Parse(messageArray[4]);
                string comment = messageArray[5];

                Review review = new Review(punctuation, comment);

                _rideRepository.AddReview(actualUser, rideId, review);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.AddReview;

                await NetworkHelper.SendMessageAsync(_clientServerSide, message);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                await NetworkHelper.SendMessageAsync(_clientServerSide, exceptionMessageToClient);
            }
        }
    }
}