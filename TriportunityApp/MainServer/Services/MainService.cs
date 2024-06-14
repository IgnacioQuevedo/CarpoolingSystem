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

namespace MainServer.Services
{
    public class MainService : AdministrativeService.AdministrativeServiceBase
    {
        private readonly RideRepository _rideRepository;
        private static readonly List<IServerStreamWriter<GrpcService.Ride>> _streamWriters = new List<IServerStreamWriter<GrpcService.Ride>>();

        public MainService()
        {
            _rideRepository = new RideRepository();
        }

        public override async Task StreamRides(StreamRidesRequest request, IServerStreamWriter<GrpcService.Ride> responseStream, ServerCallContext context)
        {
            _streamWriters.Add(responseStream);

            try
            {
                // Send the initial rides available
                var rides = _rideRepository.GetNextRides(request.Count);
                foreach (var ride in rides)
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
                }

                // Keep the stream open
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.Message}"));
            }
            finally
            {
                _streamWriters.Remove(responseStream);
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

                // Notify all connected clients
                foreach (var streamWriter in _streamWriters)
                {
                    await streamWriter.WriteAsync(grpcRide);
                }

                return new RideResponse { Status = "Ride added successfully" };
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
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.Message}"));
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
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.Message}"));
            }
        }

        public override Task<RideRatingsResponse> GetRideRatings(RideRequest request, ServerCallContext context)
        {
            try
            {
                var response = new RideRatingsResponse();
                var ratings = _rideRepository.GetDriverReviews(Guid.Parse(request.DriverId));

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
                throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.Message}"));
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
