using Grpc.Core;
using GrpcService;
using MainServer.Objects.Domain;
using MainServer.Objects.Domain.Enums;
using MainServer.Repositories;
using Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using MainServer.Exceptions;
using MainServer.Objects.Domain.VehicleModels;

namespace MainServer.Services
{
    public class MainService : AdministrativeService.AdministrativeServiceBase
    {
        private readonly RideRepository _rideRepository;
        private readonly List<IServerStreamWriter<GrpcService.Ride>> _subscribers = new List<IServerStreamWriter<GrpcService.Ride>>();
        private readonly UserRepository _userRepository;


        public MainService()
        {
            _rideRepository = new RideRepository();
            _userRepository = new UserRepository();
        }


        public override async Task StreamRides(StreamRidesRequest request, IServerStreamWriter<GrpcService.Ride> responseStream, ServerCallContext context)
        {
            int remainingRides = request.Count;
            int initialRideCount = 0;

            try
            {
                initialRideCount = _rideRepository.GetRides().Count;
            }
            catch (RideException) { }

            while (remainingRides > 0)
            {
                List<Objects.Domain.Ride> newRides = null;
                try
                {
                    var currentRides = _rideRepository.GetRides();
                    if (currentRides.Count > initialRideCount)
                    {
                        newRides = currentRides.Skip(initialRideCount).Take(currentRides.Count - initialRideCount).ToList();
                        initialRideCount = currentRides.Count;
                    }
                }
                catch (RideException) { }

                if (newRides != null && newRides.Any())
                {
                    foreach (var ride in newRides)
                    {
                        var grpcRide = new GrpcService.Ride
                        {
                            RideId = ride.Id.ToString(),
                            DriverId = ride.DriverId.ToString(),
                            Published = ride.Published,
                            InitialLocation = (int)ride.InitialLocation,
                            EndingLocation = (int)ride.EndingLocation,
                            DepartureTime = ride.DepartureTime.ToString("o"),
                            AvailableSeats = ride.AvailableSeats,
                            PricePerPerson = ride.PricePerPerson,
                            PetsAllowed = ride.PetsAllowed,
                            VehicleId = ride.VehicleId.ToString()
                        };
                        grpcRide.Passengers.AddRange(ride.Passengers.Select(p => p.ToString()));

                        await responseStream.WriteAsync(grpcRide);
                        remainingRides--;

                        if (remainingRides <= 0)
                        {
                            break;
                        }
                    }
                }
            }
        }




        public override async Task<RideResponse> AddRide(RideRequest request, ServerCallContext context)
        {
            try
            {
                var ride = ConvertToDomainRide(request);
                _rideRepository.CreateRide(ride);

                var grpcRide = new GrpcService.Ride
                {
                    RideId = ride.Id.ToString(),
                    DriverId = ride.DriverId.ToString(),
                    Published = ride.Published,
                    InitialLocation = (int)ride.InitialLocation,
                    EndingLocation = (int)ride.EndingLocation,
                    DepartureTime = ride.DepartureTime.ToString("o"),
                    AvailableSeats = ride.AvailableSeats,
                    PricePerPerson = ride.PricePerPerson,
                    PetsAllowed = ride.PetsAllowed,
                    VehicleId = ride.VehicleId.ToString()
                };
                grpcRide.Passengers.AddRange(ride.Passengers.Select(p => p.ToString()));


                return await Task.FromResult(new RideResponse { Status = "Ride added successfully" });
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.Message}"));
            }
        }

        public override Task<RideResponse> DeleteRide(RideRequest request, ServerCallContext context)
        {
            try
            {
                var rideId = Guid.Parse(request.RideId);
                _rideRepository.DeleteRide(rideId);
                return Task.FromResult(new RideResponse { Status = "Ride deleted successfully" });
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override Task<RideResponse> UpdateRide(RideRequest request, ServerCallContext context)
        {
            try
            {
                var ride = ConvertToDomainRide(request);
                ride.Id = Guid.Parse(request.RideId);
                _rideRepository.UpdateRide(ride);
                return Task.FromResult(new RideResponse { Status = "Ride updated successfully" });
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override Task<RideRatingsResponse> GetRideRatings(RideRequest request, ServerCallContext context)
        {
            try
            {
                var response = new RideRatingsResponse();
                var ratings = _rideRepository.GetDriverReviews(Guid.Parse(request.RideId));

                response.Ratings.AddRange(ratings.Select(r => new Rating
                {
                    Id = r.Id.ToString(),
                    Punctuation = r.Punctuation,
                    Comment = r.Comment
                }));

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override Task<RidesResponse> GetNextRides(RidesRequest request, ServerCallContext context)
        {
            try
            {
                var response = new RidesResponse();
                var nextRides = _rideRepository.GetNextRides(request.Count);

                response.Rides.AddRange(nextRides.Select(r =>
                {
                    var ride = new GrpcService.Ride
                    {
                        RideId = r.Id.ToString(),
                        DriverId = r.DriverId.ToString(),
                        Published = r.Published,
                        InitialLocation = (int)r.InitialLocation,
                        EndingLocation = (int)r.EndingLocation,
                        DepartureTime = r.DepartureTime.ToString("o"),
                        AvailableSeats = r.AvailableSeats,
                        PricePerPerson = r.PricePerPerson,
                        PetsAllowed = r.PetsAllowed,
                        VehicleId = r.VehicleId.ToString()
                    };
                    ride.Passengers.AddRange(r.Passengers.Select(p => p.ToString()));
                    return ride;
                }));

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<RidesResponse> GetAllRides(Empty request, ServerCallContext context)
        {
            try
            {
                var response = new RidesResponse();
                var rides = _rideRepository.GetRides();

                if (!rides.Any())
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "No rides found"));
                }

                response.Rides.AddRange(rides.Select(r =>
                {
                    var ride = new GrpcService.Ride
                    {
                        RideId = r.Id.ToString(),
                        DriverId = r.DriverId.ToString(),
                        Published = r.Published,
                        InitialLocation = (int)r.InitialLocation,
                        EndingLocation = (int)r.EndingLocation,
                        DepartureTime = r.DepartureTime.ToString("o"),
                        AvailableSeats = r.AvailableSeats,
                        PricePerPerson = r.PricePerPerson,
                        PetsAllowed = r.PetsAllowed,
                        VehicleId = r.VehicleId.ToString()
                    };
                    ride.Passengers.AddRange(r.Passengers.Select(p => p.ToString()));
                    return ride;
                }));

                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }
        public override Task<UsersResponse> GetAllUsers(Empty request, ServerCallContext context)
        {
            try
            {
                var response = new UsersResponse();
                var users = _userRepository.GetUsers();

                response.Users.AddRange(users.Select(u =>
                {
                    var user = new GrpcService.User
                    {
                        Id = u.Id.ToString(),
                        Ci = u.Ci,
                        Username = u.Username
                    };
                    return user;
                }));

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override Task<VehiclesResponse> GetAllVehiclesByUser(GetAllVehiclesByUserRequest request, ServerCallContext context)
        {
            try
            {
                var response = new VehiclesResponse();
                var vehicles = _userRepository.GetAllVehiclesByUser(Guid.Parse(request.UserId));

                response.Vehicles.AddRange(vehicles.Select(v =>
                {
                    var vehicle = new GrpcService.Vehicle
                    {
                        Id = v.Id.ToString(),
                        VehicleModel = v.VehicleModel

                    };
                    return vehicle;
                }));

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }



        private MainServer.Objects.Domain.Ride ConvertToDomainRide(RideRequest request)
        {
            return new MainServer.Objects.Domain.Ride
            {
                Id = Guid.Parse(request.RideId),
                DriverId = Guid.Parse(request.DriverId),
                Published = request.Published,
                Passengers = request.Passengers.Select(Guid.Parse).ToList(),
                InitialLocation = (CitiesEnum)request.InitialLocation,
                EndingLocation = (CitiesEnum)request.EndingLocation,
                DepartureTime = DateTime.Parse(request.DepartureTime),
                AvailableSeats = request.AvailableSeats,
                PricePerPerson = request.PricePerPerson,
                PetsAllowed = request.PetsAllowed,
                VehicleId = Guid.Parse(request.VehicleId)
            };
        }
    }
}
