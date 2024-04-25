using Client.Objects.EnumsModels;
using Client.Objects.ReviewModels;
using Client.Objects.RideModels;
using Client.Objects.UserModels;
using Common;
using System;
using System.Collections;
using System.Collections.Generic;
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
                string message = request.Driver.Id.ToString() + ";" + request.InitialLocation.ToString() + ";" +
                                 request.EndingLocation.ToString()
                                 + ";" + request.DepartureTime.ToString() + ";" + request.PricePerPerson.ToString() +
                                 ";";
                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void JoinRide(JoinRideRequest request)
        {
            try
            {
                string message = request.PassengerToJoin.Id.ToString() + ";" + request.RideId.ToString();
                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void DeleteRide(Guid id)
        {
            try
            {
                string message = id.ToString();
                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void QuitRide(QuitRideRequest request)
        {
            try
            {
                string message = request.UserToExit.Id.ToString() + ";" + request.RideId.ToString();
                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ModifyRide(ModifyRideRequest request)
        {
            try
            {
                string message = request.Id.ToString() + ";" + request.InitialLocation.ToString() + ";" +
                                 request.EndingLocation.ToString() + ";" + request.DepartureTime.ToString() + ";" +
                                 request.PricePerPerson.ToString() + ";" + request.PetsAllowed.ToString() + ";" +
                                 request.PhotoPath + ";";
                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void DisableRide(Guid id)
        {
            try
            {
                string message = id.ToString();
                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public RideClient GetRideById(Guid id)
        {
            try
            {
                string message = id.ToString();
                NetworkHelper.SendMessage(_clientSocket, message);
                string response = NetworkHelper.ReceiveMessage(_clientSocket);
                string[] rideData = response.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                return new RideClient
                {
                    Id = Guid.Parse(rideData[2]),
                    Driver = new UserClient
                    {
                        Username = rideData[3]
                    },
                    InitialLocation = (CitiesEnum)(int.Parse(rideData[6])),
                    EndingLocation = (CitiesEnum)(int.Parse(rideData[7])),
                    DepartureTime = DateTime.Parse(rideData[8]),
                    PricePerPerson = double.Parse(rideData[9]),
                    PetsAllowed = bool.Parse(rideData[10]),
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
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
                string[] ridesData = response.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                ICollection<RideClient> rides = new List<RideClient>();
                for (int i = 0; i < ridesData.Length; i += 7)
                {
                    rides.Add(new RideClient
                    {
                        Id = Guid.Parse(ridesData[i + 2]),
                        Driver = new UserClient
                        {
                            Username = ridesData[i + 3]
                        },
                        InitialLocation = (CitiesEnum)(int.Parse(ridesData[i + 4])),
                        EndingLocation = (CitiesEnum)(int.Parse(ridesData[i + 5])),
                        DepartureTime = DateTime.Parse(ridesData[i + 6]),
                        PricePerPerson = double.Parse(ridesData[i + 7]),
                        PetsAllowed = bool.Parse(ridesData[i + 8])
                    });
                }

                return rides;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public ICollection<RideClient> GetRidesFilteredByInitialLocation(CitiesEnum initialLocation)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.FilterRidesByInitialLocation +
                                 ";" + ((int)initialLocation).ToString();
                NetworkHelper.SendMessage(_clientSocket, message);
                string response = NetworkHelper.ReceiveMessage(_clientSocket);
                string[] ridesData = response.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                ICollection<RideClient> rides = new List<RideClient>();
                for (int i = 0; i < ridesData.Length; i += 7)
                {
                    rides.Add(new RideClient
                    {
                        Id = Guid.Parse(ridesData[i + 2]),
                        Driver = new UserClient
                        {
                            Username = ridesData[i + 3]
                        },
                        InitialLocation = (CitiesEnum)(int.Parse(ridesData[i + 4])),
                        EndingLocation = (CitiesEnum)(int.Parse(ridesData[i + 5])),
                        DepartureTime = DateTime.Parse(ridesData[i + 6]),
                        PricePerPerson = double.Parse(ridesData[i + 7]),
                        PetsAllowed = bool.Parse(ridesData[i + 8])
                    });
                }

                return rides;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public ICollection<RideClient> GetRidesFilteredByEndingLocation(CitiesEnum endingLocation)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.FilterRidesByEndingLocation +
                                 ";" + ((int)endingLocation).ToString();
                NetworkHelper.SendMessage(_clientSocket, message);
                string response = NetworkHelper.ReceiveMessage(_clientSocket);
                string[] ridesData = response.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                ICollection<RideClient> rides = new List<RideClient>();
                for (int i = 0; i < ridesData.Length; i += 7)
                {
                    rides.Add(new RideClient
                    {
                        Id = Guid.Parse(ridesData[i + 2]),
                        Driver = new UserClient
                        {
                            Username = ridesData[i + 3]
                        },
                        InitialLocation = (CitiesEnum)(int.Parse(ridesData[i + 4])),
                        EndingLocation = (CitiesEnum)(int.Parse(ridesData[i + 5])),
                        DepartureTime = DateTime.Parse(ridesData[i + 6]),
                        PricePerPerson = double.Parse(ridesData[i + 7]),
                        PetsAllowed = bool.Parse(ridesData[i + 8])
                    });
                }

                return rides;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public ICollection<ReviewClient> GetDriverReviews(Guid rideId)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetDriverReviews + ";" +
                                 rideId.ToString();
                NetworkHelper.SendMessage(_clientSocket, message);
                string response = NetworkHelper.ReceiveMessage(_clientSocket);
                string[] reviewsData = response.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                ICollection<ReviewClient> reviews = new List<ReviewClient>();
                for (int i = 0; i < reviewsData.Length; i += 3)
                {
                    reviews.Add(new ReviewClient
                    {
                        Id = Guid.Parse(reviewsData[i + 2]),
                        Punctuation = int.Parse(reviewsData[i + 3]),
                        Comment = reviewsData[i + 4]
                    });
                }

                return reviews;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public void UpdateRide(RideClient rideWithUpdates)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.EditRide + ";" +
                                 rideWithUpdates.Id.ToString() + ";" + rideWithUpdates.AvailableSeats.ToString() +
                                 ";" + rideWithUpdates.DepartureTime.ToString() + ";" +
                                 rideWithUpdates.EndingLocation.ToString() +
                                 ";" + rideWithUpdates.InitialLocation.ToString() + ";" +
                                 rideWithUpdates.PricePerPerson.ToString()
                                 + ";" + rideWithUpdates.PetsAllowed.ToString();
                NetworkHelper.SendMessage(_clientSocket, message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public ICollection<RideClient> GetAllRides()
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetAllRides;
                NetworkHelper.SendMessage(_clientSocket, message);
                string response = NetworkHelper.ReceiveMessage(_clientSocket);
                string[] ridesData = response.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                ICollection<RideClient> rides = new List<RideClient>();
                for (int i = 0; i < ridesData.Length; i += 7)
                {
                    rides.Add(new RideClient
                    {
                        Id = Guid.Parse(ridesData[i + 2]),
                        Driver = new UserClient
                        {
                            Username = ridesData[i + 3]
                        },
                        InitialLocation = (CitiesEnum)(int.Parse(ridesData[i + 4])),
                        EndingLocation = (CitiesEnum)(int.Parse(ridesData[i + 5])),
                        DepartureTime = DateTime.Parse(ridesData[i + 6]),
                        PricePerPerson = double.Parse(ridesData[i + 7]),
                        PetsAllowed = bool.Parse(ridesData[i + 8])
                    });
                }

                return rides;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public void GetCarImageById(Guid rideSelectedId)
        {
            RideClient ride = GetRideById(rideSelectedId);

            string getImage = ProtocolConstants.Request + ";" + CommandsConstraints.GetCarImage + ";" + ride.Driver.Id.ToString() +
                              ";" + ride.VehicleId.ToString();

            NetworkHelper.SendMessage(_clientSocket, getImage);

            NetworkHelper.ReceiveImage(_clientSocket);
            Console.WriteLine("Image received!, see it at your destination folder.");
        }
    }
}