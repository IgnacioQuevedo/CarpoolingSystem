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

        public void CreateRide(string [] messageArray)
        {
            try
            {
                Guid id = Guid.Parse(messageArray[2]);
                CitiesEnum initialLocation = (CitiesEnum)int.Parse(messageArray[3]);
                CitiesEnum endingLocation = (CitiesEnum)int.Parse(messageArray[4]);
                DateTime departureTime = DateTime.Parse(messageArray[5]);
                int availableSeats = int.Parse(messageArray[6]);
                double price = double.Parse(messageArray[7]);
                bool pets = bool.Parse(messageArray[8]);
                Guid vehicleId = Guid.Parse(messageArray[9]);


                List<Guid> passengers = new List<Guid>();

                foreach (var passenger in messageArray[10].Split(new string[] { "," },
                                   StringSplitOptions.RemoveEmptyEntries))
                {
                    passengers.Add(Guid.Parse(passenger));
                }

                Ride ride = new Ride(id, passengers, initialLocation, endingLocation, departureTime, availableSeats, price, pets, vehicleId);

                _rideRepository.CreateRide(ride);
            } catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }

        public void JoinRide(string[] messageArray)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                Guid userId = Guid.Parse(messageArray[3]);

                _rideRepository.JoinRide(rideId, userId);
            } catch (Exception e)
            {
               throw new Exception("Error: " + e.Message);
            }
         
        }

        public void EditRide(string[] messageArray)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                CitiesEnum initialLocation = (CitiesEnum)int.Parse(messageArray[3]);
                CitiesEnum endingLocation = (CitiesEnum)int.Parse(messageArray[4]);
                DateTime departureTime = DateTime.Parse(messageArray[5]);
                int availableSeats = int.Parse(messageArray[6]);
                double price = double.Parse(messageArray[7]);
                bool pets = bool.Parse(messageArray[8]);
                Guid vehicleId = Guid.Parse(messageArray[9]);

                List<Guid> passengers = new List<Guid>();

                foreach (var passenger in messageArray[10].Split(new string[] { "," },
                                       StringSplitOptions.RemoveEmptyEntries))
                {
                    passengers.Add(Guid.Parse(passenger));
                }

                Ride ride = new Ride(rideId, passengers, initialLocation, endingLocation, departureTime, availableSeats, price, pets, vehicleId);

                _rideRepository.UpdateRide(ride);
            } catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }   
        }

        public void DeleteRide(string[] messageArray)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                _rideRepository.DeleteRide(rideId);
              
            } catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }

        public void QuitRide(string[] messageArray)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                Guid userId = Guid.Parse(messageArray[3]);

                _rideRepository.QuitRide(rideId, userId);
            } catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }

        public void FilterRidesByInitialLocation(string[] messageArray)
        {
            try
            {
                CitiesEnum initialLocation = (CitiesEnum)int.Parse(messageArray[2]);

                ICollection<Ride> rides = _rideRepository.FilterByInitialLocation(initialLocation);

                

                string response = ProtocolConstants.Response + ";" + CommandsConstraints.FilterRidesByInitialLocation + ";";

                foreach (var ride in rides)
                {
                    response += ride.Id + ":" + ride.DriverId + ":" + ":";

                    foreach (var passenger in ride.Passengers)
                    {
                        response += passenger + ",";
                    }

                    response += ":" +  ride.InitialLocation +
                        ":" + ride.EndingLocation + ":" + ride.DepartureTime + ":" + ride.AvailableSeats + ":" + ride.PricePerPerson + ":" + ride.PetsAllowed + ":"
                        + ride.VehicleId;
                }

                NetworkHelper.SendMessage(_clientSocket, response);

            } catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }

        public void FilterRidesByEndingLocation(string[] messageArray)
        {
            try
            {
                CitiesEnum endingLocation = (CitiesEnum)int.Parse(messageArray[2]);

                ICollection<Ride> rides = _rideRepository.FilterByDestination(endingLocation.ToString());

                string response = ProtocolConstants.Response + ";" + CommandsConstraints.FilterRidesByEndingLocation + ";";

                foreach (var ride in rides)
                {
                    response += ride.Id + ":" + ride.DriverId + ":" + ":";

                    foreach (var passenger in ride.Passengers)
                    {
                        response += passenger + ",";
                    }

                    response += ":" + ride.InitialLocation +
                        ":" + ride.EndingLocation + ":" + ride.DepartureTime + ":" + ride.AvailableSeats + ":" + ride.PricePerPerson + ":" + ride.PetsAllowed + ":"
                        + ride.VehicleId;
                }

                NetworkHelper.SendMessage(_clientSocket, response);

            } catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }

        public void  FilterRidesByPrice(string[] messageArray)
        {
            try
            {
                double minPrice = double.Parse(messageArray[2]);
                double maxPrice = double.Parse(messageArray[3]);

                ICollection<Ride> rides = _rideRepository.FilterByPrice(minPrice, maxPrice);

                string response = ProtocolConstants.Response + ";" + CommandsConstraints.FilterRidesByPrice + ";";

                foreach (var ride in rides)
                {
                    response += ride.Id + ":" + ride.DriverId + ":" + ":";

                    foreach (var passenger in ride.Passengers)
                    {
                        response += passenger + ",";
                    }

                    response += ":" + ride.InitialLocation +
                        ":" + ride.EndingLocation + ":" + ride.DepartureTime + ":" + ride.AvailableSeats + ":" + ride.PricePerPerson + ":" + ride.PetsAllowed + ":"
                        + ride.VehicleId;
                }

                NetworkHelper.SendMessage(_clientSocket, response);

            } catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
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
                    response += ride.Id + ":" + ride.DriverId + ":" + ":";

                    foreach (var passenger in ride.Passengers)
                    {
                        response += passenger + ",";
                    }

                    response += ":" + ride.InitialLocation +
                        ":" + ride.EndingLocation + ":" + ride.DepartureTime + ":" + ride.AvailableSeats + ":" + ride.PricePerPerson + ":" + ride.PetsAllowed + ":"
                        + ride.VehicleId;
                }

                NetworkHelper.SendMessage(_clientSocket, response);

            } catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
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

            } catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }

        public void GetDriverReviews(string[] messageArray)
        {
            try
            {
                Guid userId = Guid.Parse(messageArray[2]);
                User user = _userRepository.GetUserById(userId);

                if (user.DriverAspects == null)
                {
                    throw new Exception("User is not a driver");
                }

                string response = ProtocolConstants.Response + ";" + CommandsConstraints.GetDriverReviews + ";";

                foreach (var review in user.DriverAspects.Reviews)
                {
                    response += review.Id + ":" + review.Punctuation + ":" + review.Comment + ",";
                }

                NetworkHelper.SendMessage(_clientSocket, response);

            } catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }

        public void DisableRide(string[] messageArray)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                _rideRepository.DisablePublishedRide(rideId);
            } catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }

        public void GetRideById(string[] messageArray)
        {
            try
            {
                Guid rideId = Guid.Parse(messageArray[2]);
                Ride ride = _rideRepository.GetRideById(rideId);

                string response = ProtocolConstants.Response + ";" + CommandsConstraints.GetRideById + ";";

                response += ride.Id + ":" + ride.DriverId + ":" + ":";

                foreach (var passenger in ride.Passengers)
                {
                    response += passenger + ",";
                }

                response += ":" + ride.InitialLocation +
                    ":" + ride.EndingLocation + ":" + ride.DepartureTime + ":" + ride.AvailableSeats + ":" + ride.PricePerPerson + ":" + ride.PetsAllowed + ":"
                    + ride.VehicleId;

                NetworkHelper.SendMessage(_clientSocket, response);

            } catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }

    }
}