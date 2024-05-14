using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Client.Objects.RideModels;
using Common;
using Server.Exceptions;
using Server.Objects.Domain;
using Server.Objects.Domain.Enums;
using Server.Objects.Domain.UserModels;
using Server.Objects.Domain.VehicleModels;
using Server.Repositories;

namespace Server.Controllers
{
    public class RideController
    {
        private static TcpClient _clientServerSide;
        private static RideRepository _rideRepository = new RideRepository();
        private static UserRepository _userRepository = new UserRepository();

        public RideController(TcpClient clientServerSide)
        {
            _clientServerSide = clientServerSide;
        }

        public void CreateRide(string[] messageArray)
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

                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }

        public void GetRidesByUser(string[] messageArray)
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

                NetworkHelper.SendMessage(_clientSocket, response);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }

        public void JoinRide(string[] messageArray)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                Guid userId = Guid.Parse(messageArray[3]);

                _rideRepository.JoinRide(userId, rideId);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.JoinRide;

                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }

        public void EditRide(string[] messageArray)
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

                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }

        public void DeleteRide(string[] messageArray)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                _rideRepository.DeleteRide(rideId);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.DeleteRide;

                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }

        public void QuitRide(string[] messageArray)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                Guid userId = Guid.Parse(messageArray[3]);

                _rideRepository.QuitRide(rideId, userId);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.QuitRide;
                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }

        public void GetAllRides()
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

                NetworkHelper.SendMessage(_clientSocket, response);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }

        public void GetCarImage(string[] messageArray)
        {
            try
            {
                Guid userId = Guid.Parse(messageArray[2]);
                Guid rideId = Guid.Parse(messageArray[3]);
                Ride rideToFound = _rideRepository.GetRideById(rideId);
                Guid vehicleId = rideToFound.VehicleId;
                Vehicle vehicle = _userRepository.GetVehicleById(userId, vehicleId);

                NetworkHelper.SendImage(_clientSocket, vehicle.ImageAllocatedAtServer);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }

        public void DisableRide(string[] messageArray)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                _rideRepository.DisablePublishedRide(rideId);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.DisableRide;
                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception e)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + e.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }

        public void FilterRidesByPrice(string[] messageArray)
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

                NetworkHelper.SendMessage(_clientSocket, response);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }

        public void GetDriverReviews(string[] messageArray)
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

                NetworkHelper.SendMessage(_clientSocket, response);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }


        public void GetRideById(string[] messageArray)
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

                NetworkHelper.SendMessage(_clientSocket, response);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }

        public void AddReview(string[] messageArray)
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

                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" +
                                                  CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }
    }
}