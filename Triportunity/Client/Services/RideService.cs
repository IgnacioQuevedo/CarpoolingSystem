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
using System.Threading;
using System.Threading.Tasks;

namespace Client.Services
{
    public class RideService
    {
        public RideService()
        {
        }

        public async Task CreateRideAsync(TcpClient client, CreateRideRequest request)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.CreateRide + ";" +
                                 request.DriverId + ";";

                message += request.InitialLocation + ";" + request.EndingLocation + ";" + request.DepartureTime +
                           ";" + request.AvailableSeats + ";" + request.PricePerPerson + ";" + request.PetsAllowed
                           + ";" + request.VehicleId;

                await NetworkHelper.SendMessageAsync(client, message);

                string messageReceiveed = await NetworkHelper.ReceiveMessageAsync(client);

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

        public async Task JoinRideAsync(TcpClient client, JoinRideRequest request)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.JoinRide + ";" +
                                 request.RideId + ";" + request.PassengerToJoin;
                await NetworkHelper.SendMessageAsync(client, message);

                string messageReceived = await NetworkHelper.ReceiveMessageAsync(client);

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

        public async Task DeleteRideAsync(TcpClient client, Guid id)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.DeleteRide + ";" + id;
                await NetworkHelper.SendMessageAsync(client, message);

                string messageReceived = await NetworkHelper.ReceiveMessageAsync(client);

                string[] messageArrayResponse =
                    messageReceived.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

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

        public async Task QuitRideAsync(TcpClient client, QuitRideRequest request)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.QuitRide + ";" + 
                                 request.UserToExit.Id+ ";" + request.RideId;

                await NetworkHelper.SendMessageAsync(client, message);
                string messageReceived = await NetworkHelper.ReceiveMessageAsync(client);

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

        public async Task ModifyRideAsync(TcpClient client, ModifyRideRequest request)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.EditRide + ";" +
                                 request.RideId + ";";
                message += request.InitialLocation + ";" + request.EndingLocation + ";" +
                           request.DepartureTime + ";" + request.AvailableSeats + ";" + request.PricePerPerson
                           + ";" + request.PetsAllowed + ";" + request.VehicleId + ";" + request.DriverId;
                await NetworkHelper.SendMessageAsync(client, message);

                string messageReceived = await NetworkHelper.ReceiveMessageAsync(client);

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

        public async Task DisableRideAsync(TcpClient client, Guid id)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.DisableRide + ";" +
                                 id;
                await NetworkHelper.SendMessageAsync(client, message);

                string messageReceived = await NetworkHelper.ReceiveMessageAsync(client);

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

        public async Task<RideClient> GetRideByIdAsync(TcpClient client, Guid id)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetRideById + ";" +
                                   id;
                await NetworkHelper.SendMessageAsync(client , message);

                string response = await NetworkHelper.ReceiveMessageAsync(client);

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

        public async Task<ICollection<RideClient>> GetRidesFilteredByPriceAsync(TcpClient client, double minPrize, double maxPrice)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.FilterRidesByPrice + ";" +
                                 minPrize + ";" + maxPrice;
                await NetworkHelper.SendMessageAsync(client, message);

                string response = await NetworkHelper.ReceiveMessageAsync(client);
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


        public async Task<ICollection<RideClient>> GetRidesByUserAsync(TcpClient client, Guid userLoggedId)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetRidesByUser + ";" + userLoggedId;
                await NetworkHelper.SendMessageAsync(client, message);

                string response = await NetworkHelper.ReceiveMessageAsync(client);

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


        public async Task<ICollection<RideClient>> GetAllRidesAsync(TcpClient client)
        {
            try
            {
                ICollection<RideClient> rides = new List<RideClient>();
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetAllRides;

                await NetworkHelper.SendMessageAsync(client, message);

                string response = await NetworkHelper.ReceiveMessageAsync(client);

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

        public async Task<string> GetCarImageByIdAsync(TcpClient client, Guid userId, Guid rideSelectedId, CancellationToken token)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetCarImage + ";" + userId +
                                 ";" +
                                 rideSelectedId;
                await NetworkHelper.SendMessageAsync(client, message);


                string imagePath = await NetworkHelper.ReceiveImageAsync(client, token);

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

        public async Task AddReviewAsync(TcpClient client, Guid actualUserId, ReviewClient request)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.AddReview + ";" + actualUserId + ";" + request.DriverId + ";" + request.Punctuation + ";" + request.Comment;
                await NetworkHelper.SendMessageAsync(client, message);

                string messageReceived = await NetworkHelper.ReceiveMessageAsync(client);

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


        public async Task<ICollection<ReviewClient>> GetDriverReviews(TcpClient client ,Guid rideId)
        {
            try
            {
                string message = ProtocolConstants.Request + ";" + CommandsConstraints.GetDriverReviews + ";" +
                                 rideId;
                await NetworkHelper.SendMessageAsync(client, message);

                string response = await NetworkHelper.ReceiveMessageAsync(client);

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