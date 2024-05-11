using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Client.Objects.RideModels;
using Common;
using Server.Objects.Domain;
using Server.Objects.Domain.Enums;
using Server.Objects.Domain.UserModels;
using Server.Objects.Domain.VehicleModels;
using Server.Repositories;

namespace Server.Controllers
{
    public class RideController
    {
        private static Socket _clientSocket;
        private static RideRepository _rideRepository = new RideRepository();
        private static UserRepository _userRepository = new UserRepository();

        public RideController(Socket clientSocket)
        {
            _clientSocket = clientSocket;
        }

        public void CreateRide(string[] messageArray)
        {
            try
            {
                List<Guid> passengers = new List<Guid>();

                Guid id = Guid.Parse(messageArray[2]);

                CitiesEnum initialLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), messageArray[4]);
                CitiesEnum endingLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), messageArray[5]);
                DateTime departureTime = DateTime.Parse(messageArray[6]);
                int availableSeats = int.Parse(messageArray[7]);
                double price = double.Parse(messageArray[8]);
                bool pets = bool.Parse(messageArray[9]);
                Guid vehicleId = Guid.Parse(messageArray[10]);




                Ride ride = new Ride(id, passengers, initialLocation, endingLocation, departureTime, availableSeats,
                    price, pets, vehicleId);

                _rideRepository.CreateRide(ride);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.CreateRide + ";" + ride.Id;

                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
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
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
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
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }

        public void EditRide(string[] messageArray)
        {
            try
            {
                List<Guid> passengers = new List<Guid>();

                Guid id = Guid.Parse(messageArray[2]);

                if (messageArray[3] != "#")
                {
                    foreach (var passenger in messageArray[10].Split(new string[] { "," },
                                 StringSplitOptions.RemoveEmptyEntries))
                    {
                        passengers.Add(Guid.Parse(passenger));
                    }
                }

                CitiesEnum initialLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), messageArray[4]);
                CitiesEnum endingLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), messageArray[5]);
                DateTime departureTime = DateTime.Parse(messageArray[6]);
                int availableSeats = int.Parse(messageArray[7]);
                double price = double.Parse(messageArray[8]);
                bool pets = bool.Parse(messageArray[9]);
                Guid vehicleId = Guid.Parse(messageArray[10]);

                Ride ride = new Ride(id, passengers, initialLocation, endingLocation, departureTime, availableSeats,
                    price, pets, vehicleId);

                _rideRepository.UpdateRide(ride);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
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
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
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
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
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
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
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

                NetworkHelper.SendImage(_clientSocket, vehicle.ImageAllocatedAtAServer);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }

        public void DisableRide(string[] messageArray)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                _rideRepository.DisablePublishedRide(rideId);

            }
            catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }

        public void FilterRidesByPrice(string[] messageArray)
        {
            try
            {
                double minPrice = double.Parse(messageArray[2]);
                double maxPrice = double.Parse(messageArray[3]);

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
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
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
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
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
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }
        public void AddReview(string[] messageArray)
        {
            try
            {
                Guid userId = Guid.Parse(messageArray[2]);
                double punctuation = double.Parse(messageArray[3]);
                string comment = messageArray[4];

                Review review = new Review(punctuation, comment);

                _rideRepository.AddReview(userId, review);

                string message = ProtocolConstants.Response + ";" + CommandsConstraints.AddReview;

                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception exceptionCaught)
            {
                string exceptionMessageToClient = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + exceptionCaught.Message;
                NetworkHelper.SendMessage(_clientSocket, exceptionMessageToClient);
            }
        }



    }
}