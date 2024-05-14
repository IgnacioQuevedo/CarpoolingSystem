using Client.Objects.EnumsModels;
using Client.Objects.ReviewModels;
using Client.Objects.RideModels;
using Client.Objects.UserModels;
using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Client.Services
{
    public class RideService
    {
        private static Socket _clientSocket;

        public RideService(Socket socket)
        {
            _clientSocket = socket;
        }

        public void CreateRide(CreateRideRequest request)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.CreateRide + ";" +
                                 request.DriverId + ";";

                message += request.InitialLocation + ";" + request.EndingLocation + ";" + request.DepartureTime +
                           ";" + request.AvailableSeats + ";" + request.PricePerPerson + ";" + request.PetsAllowed
                           + ";" + request.VehicleId;

                NetworkHelper.SendMessage(_clientSocket, message);

                string messageReceiveed = NetworkHelper.ReceiveMessage(_clientSocket);

                string[] messageArrayResponse =
                    messageReceiveed.Split(new string[] { ";" }, StringSplitOptions.None);

                if (messageArrayResponse[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(messageArrayResponse[2]);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void JoinRide(JoinRideRequest request)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.JoinRide + ";" +
                                 request.RideId + ";" + request.PassengerToJoin;
                NetworkHelper.SendMessage(_clientSocket, message);

                string messageReceived = NetworkHelper.ReceiveMessage(_clientSocket);

                string[] messageArrayResponse =
                    messageReceived.Split(new string[] { ";" }, StringSplitOptions.None);

                if (messageArrayResponse[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(messageArrayResponse[2]);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void DeleteRide(Guid id)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.DeleteRide + ";" + id;
                NetworkHelper.SendMessage(_clientSocket, message);

                string messageRecevied = NetworkHelper.ReceiveMessage(_clientSocket);

                string[] messageArrayResponse =
                    messageRecevied.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                if (messageArrayResponse[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(messageArrayResponse[2]);
                }
                else if (messageArrayResponse[0] == ProtocolConstants.Response)
                {
                    Console.WriteLine("Ride deleted successfully");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void QuitRide(QuitRideRequest request)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.QuitRide + ";" + 
                                 request.UserToExit.Id+ ";" + request.RideId;

                NetworkHelper.SendMessage(_clientSocket, message);
                string messageReceived = NetworkHelper.ReceiveMessage(_clientSocket);

                string[] messageArrayResponse =
                    messageReceived.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                if (messageArrayResponse[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(messageArrayResponse[2]);
                }
            }
            catch (Exception exceptionCaught)
            {
                throw new Exception(exceptionCaught.Message);
            }
        }

        public void ModifyRide(ModifyRideRequest request)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.EditRide + ";" +
                                 request.RideId + ";";
                message += request.InitialLocation + ";" + request.EndingLocation + ";" +
                           request.DepartureTime + ";" + request.AvailableSeats + ";" + request.PricePerPerson
                           + ";" + request.PetsAllowed + ";" + request.VehicleId + ";" + request.DriverId;
                NetworkHelper.SendMessage(_clientSocket, message);

                string messageReceived = NetworkHelper.ReceiveMessage(_clientSocket);

                string[] messageArrayResponse =
                    messageReceived.Split(new string[] { ";" }, StringSplitOptions.None);

                if (messageArrayResponse[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(messageArrayResponse[2]);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void DisableRide(Guid id)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.DisableRide + ";" +
                                 id;
                NetworkHelper.SendMessage(_clientSocket, message);

                string messageReceived = NetworkHelper.ReceiveMessage(_clientSocket);

                string[] messageArrayResponse =
                    messageReceived.Split(new string[] { ";" }, StringSplitOptions.None);

                if (messageArrayResponse[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(messageArrayResponse[2]);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public RideClient GetRideById(Guid id)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetRideById + ";" +
                                   id;
                NetworkHelper.SendMessage(_clientSocket, message);

                string response = NetworkHelper.ReceiveMessage(_clientSocket);

                string[] rideData = response.Split(new string[] { ";" }, StringSplitOptions.None);

                if (rideData[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(rideData[2]);
                }

                string[] rideArray = rideData[2].Split(new string[] { ":" }, StringSplitOptions.None);

                ICollection<Guid> passengers = new List<Guid>();

                foreach (var passenger in rideArray[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    passengers.Add(Guid.Parse(passenger));
                }

                DateTime departureTime = DateTime.Parse(rideArray[5] + ":" + rideArray[6] + ":" + rideArray[7]);

                RideClient ride = new RideClient
                {
                    Id = Guid.Parse(rideArray[0]),
                    DriverId = Guid.Parse(rideArray[1]),
                    Passengers = passengers,
                    InitialLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), rideArray[3]),
                    EndingLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), rideArray[4]),
                    DepartureTime = departureTime,
                    AvailableSeats = int.Parse(rideArray[8]),
                    PricePerPerson = double.Parse(rideArray[9]),
                    PetsAllowed = bool.Parse(rideArray[10]),
                    VehicleId = Guid.Parse(rideArray[11])
                };
                return ride;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ICollection<RideClient> GetRidesFilteredByPrice(double minPrize, double maxPrice)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.FilterRidesByPrice + ";" +
                                 minPrize.ToString() + ";" + maxPrice.ToString();
                NetworkHelper.SendMessage(_clientSocket, message);

                string response = NetworkHelper.ReceiveMessage(_clientSocket);
                string[] ridesData = response.Split(new string[] { ";" }, StringSplitOptions.None);
                ICollection<RideClient> rides = new List<RideClient>();
                if (ridesData[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(ridesData[2]);
                }

                string[] allRides = ridesData[2].Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < allRides.Length; i++)
                {
                    string[] rideInfo = allRides[i].Split(new string[] { ":" }, StringSplitOptions.None);
                    ICollection<Guid> passengers = new List<Guid>();

                    string[] passengersString =
                        rideInfo[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var passenger in passengersString)
                    {
                        passengers.Add(Guid.Parse(passenger));
                    }


                    DateTime departureTime = DateTime.Parse(rideInfo[5] + " " + rideInfo[6] + " " + rideInfo[7]);

                    rides.Add(new RideClient
                    {
                        Id = Guid.Parse(rideInfo[0]),
                        DriverId = Guid.Parse(rideInfo[1]),
                        Passengers = passengers,
                        InitialLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), rideInfo[3]),
                        EndingLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), rideInfo[4]),
                        DepartureTime = departureTime,
                        AvailableSeats = int.Parse(rideInfo[8]),
                        PricePerPerson = double.Parse(rideInfo[9]),
                        PetsAllowed = bool.Parse(rideInfo[10]),
                        VehicleId = Guid.Parse(rideInfo[11])
                    });
                }

                return rides;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public ICollection<RideClient> GetRidesByUser(Guid userLoggedId)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetRidesByUser + ";" + userLoggedId;
                NetworkHelper.SendMessage(_clientSocket, message);

                string response = NetworkHelper.ReceiveMessage(_clientSocket);

                string[] ridesData = response.Split(new string[] { ";" }, StringSplitOptions.None);

                if (ridesData[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(ridesData[2]);
                }

                ICollection<RideClient> rides = new List<RideClient>();

                string[] allRides = ridesData[2].Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < allRides.Length; i++)
                {
                    string[] rideInfo = allRides[i].Split(new string[] { ":" }, StringSplitOptions.None);
                    ICollection<Guid> passengers = new List<Guid>();

                    foreach (var passenger in rideInfo[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        passengers.Add(Guid.Parse(passenger));
                    }

                    DateTime departureTime = DateTime.Parse(rideInfo[5] + ":" + rideInfo[6] + ":" + rideInfo[7]);

                    rides.Add(new RideClient
                    {
                        Id = Guid.Parse(rideInfo[0]),
                        DriverId = Guid.Parse(rideInfo[1]),
                        Passengers = passengers,
                        InitialLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), rideInfo[3]),
                        EndingLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), rideInfo[4]),
                        DepartureTime = departureTime,
                        AvailableSeats = int.Parse(rideInfo[8]),
                        PricePerPerson = double.Parse(rideInfo[9]),
                        PetsAllowed = bool.Parse(rideInfo[10]),
                        VehicleId = Guid.Parse(rideInfo[11])
                    });
                }

                return rides;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public ICollection<RideClient> GetAllRides()
        {
            try
            {
                ICollection<RideClient> rides = new List<RideClient>();
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetAllRides;

                NetworkHelper.SendMessage(_clientSocket, message);

                string response = NetworkHelper.ReceiveMessage(_clientSocket);

                string[] ridesData = response.Split(new string[] { ";" }, StringSplitOptions.None);

                if (ridesData[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(ridesData[2]);
                }


                string[] allRides = ridesData[2].Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < allRides.Length; i++)
                {
                    string[] rideInfo = allRides[i].Split(new string[] { ":" }, StringSplitOptions.None);
                    ICollection<Guid> passengers = new List<Guid>();

                    string[] passengersString =
                                     rideInfo[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var passenger in passengersString)
                    {
                        passengers.Add(Guid.Parse(passenger));
                    }


                    DateTime departureTime = DateTime.Parse(rideInfo[5] + " " + rideInfo[6] + " " + rideInfo[7]);

                    rides.Add(new RideClient
                    {
                        Id = Guid.Parse(rideInfo[0]),
                        DriverId = Guid.Parse(rideInfo[1]),
                        Passengers = passengers,
                        InitialLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), rideInfo[3]),
                        EndingLocation = (CitiesEnum)Enum.Parse(typeof(CitiesEnum), rideInfo[4]),
                        DepartureTime = departureTime,
                        AvailableSeats = int.Parse(rideInfo[8]),
                        PricePerPerson = double.Parse(rideInfo[9]),
                        PetsAllowed = bool.Parse(rideInfo[10]),
                        VehicleId = Guid.Parse(rideInfo[11])
                    });
                }

                return rides;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string GetCarImageById(Guid userId, Guid rideSelectedId)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetCarImage + ";" + userId +
                                 ";" +
                                 rideSelectedId;
                NetworkHelper.SendMessage(_clientSocket, message);


                string imagePath = NetworkHelper.ReceiveImage(_clientSocket);

                string[] messageArray = imagePath.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                if (messageArray[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(messageArray[2]);
                }

                return imagePath;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void AddReview(Guid actualUserId, ReviewClient request)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.AddReview + ";" + actualUserId + ";" + request.DriverId + ";" + request.Punctuation + ";" + request.Comment;
                NetworkHelper.SendMessage(_clientSocket, message);

                string messageReceiveed = NetworkHelper.ReceiveMessage(_clientSocket);

                string[] messageArrayResponse =
                    messageReceiveed.Split(new string[] { ";" }, StringSplitOptions.None);

                if (messageArrayResponse[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(messageArrayResponse[2]);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public ICollection<ReviewClient> GetDriverReviews(Guid rideId)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetDriverReviews + ";" +
                                 rideId;
                NetworkHelper.SendMessage(_clientSocket, message);

                string response = NetworkHelper.ReceiveMessage(_clientSocket);

                string[] reviewsData = response.Split(new string[] { ";" }, StringSplitOptions.None);

                if (reviewsData[0] == ProtocolConstants.Exception)
                {
                    throw new Exception(reviewsData[2]);
                }

                ICollection<ReviewClient> reviews = new List<ReviewClient>();

                string[] allReviews = reviewsData[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < allReviews.Length; i++)
                {
                    string[] reviewInfo = allReviews[i].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                    reviews.Add(new ReviewClient
                    {
                        DriverId = Guid.Parse(reviewInfo[0]),
                        Punctuation = double.Parse(reviewInfo[1]),
                        Comment = reviewInfo[2],
                    });
                }

                return reviews;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}