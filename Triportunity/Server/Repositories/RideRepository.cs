using Server.DataContext;
using Server.Objects.Domain;
using System.Collections.Generic;
using System;
using Server.Objects.DTOs.RideModelDtos;
using System.Linq;
using Client.Services;
using Server.Exceptions;
using Server.Objects.Domain.UserModels;
using Server.Objects.Domain.Enums;
using Server.Objects.Domain.VehicleModels;

namespace Server.Repositories
{
    public class RideRepository
    {
        public static UserRepository _userRepository = new UserRepository();

        public void CreateRide(Ride rideToAdd)
        {
            LockManager.StartWriting();
            MemoryDatabase.GetInstance().Rides.Add(rideToAdd);
            LockManager.StopWriting();
        }

        public void JoinRide(Guid userId, Guid rideId)
        {
            LockManager.StartWriting();
            Ride rideToJoin = GetRideById(rideId);
            User user = _userRepository.GetUserById(userId);

            ValidateJoinRide(userId, rideToJoin);
            rideToJoin.Passengers.Add(userId);
            rideToJoin.AvailableSeats--;
            LockManager.StopWriting();
        }

        private void ValidateJoinRide(Guid user, Ride rideToJoin)
        {
            string exceptionMessage = "";

            if (rideToJoin.AvailableSeats < 1) exceptionMessage = "No are no available seats";
            if (rideToJoin.Passengers.Contains(user)) exceptionMessage = "User is already in the ride";
            if (rideToJoin.DepartureTime <= DateTime.Now)
                exceptionMessage = "Cannot join a ride that has already departed";

            if (exceptionMessage != "") throw new RideException(exceptionMessage);
        }

        public Ride GetRideById(Guid rideId)
        {
            var rideToFind = MemoryDatabase.GetInstance().Rides.FirstOrDefault(ride => ride.Id == rideId);

            if (rideToFind == null)
            {
                throw new RideException("Ride not found");
            }

            return rideToFind;
        }

        public void QuitRide(Guid userId, Guid rideId)
        {
            LockManager.StartWriting();
            Ride rideToQuit = GetRideById(rideId);
            User user = _userRepository.GetUserById(userId);

            if (!rideToQuit.Passengers.Contains(userId))
            {
                throw new RideException("User is not in the ride");
            }

            if (rideToQuit.DepartureTime <= DateTime.Now)
            {
                throw new RideException("Cannot quit the ride as the departure time has passed.");
            }

            rideToQuit.Passengers.Remove(userId);
            rideToQuit.AvailableSeats++;
            LockManager.StopWriting();
        }

        public ICollection<Ride> GetRides()
        {
            ICollection<Ride> rides = new List<Ride>();
            rides = MemoryDatabase.GetInstance().Rides;

            ICollection<Ride> availableRides = new List<Ride>();

            foreach (var ride in rides)
            {
                if (ride.Published == true && ride.DepartureTime > DateTime.Now && ride.AvailableSeats > 0)
                {
                    availableRides.Add(ride);
                }
            }

            if (rides.Count == 0)
            {
                throw new RideException("No rides found");
            }

            return rides;
        }

        public void DeleteRide(Guid rideId)
        {
            Ride rideToDelete = new Ride();
            rideToDelete = GetRideById(rideId);
            
            LockManager.StartWriting();
            MemoryDatabase.GetInstance().Rides.Remove(rideToDelete);
            LockManager.StopWriting();
        }

        public void DisablePublishedRide(Guid rideId)
        {
            LockManager.StartWriting();
            Ride rideToCancel = GetRideById(rideId);
            rideToCancel.Published = false;
            LockManager.StopWriting();
        }

        public ICollection<Ride> FilterByPrice(double minPrice, double maxPrice)
        {
            LockManager.StartReading();
            ICollection<Ride> filteredRides = new List<Ride>();
            var rides = GetRides();

            filteredRides = rides
                .Where(ride => ride.PricePerPerson >= minPrice && ride.PricePerPerson <= maxPrice)
                .ToList();

            LockManager.StopReading();
            return filteredRides;
        }

        public ICollection<Ride> FilterByInitialLocation(CitiesEnum initialLocation)
        {
            LockManager.StartReading();
            ICollection<Ride> filteredRides = new List<Ride>();
            filteredRides = MemoryDatabase.GetInstance().Rides
                .Where(ride => ride.InitialLocation.Equals(initialLocation))
                .ToList();
            LockManager.StopReading();
            return filteredRides;
        }

        public ICollection<Ride> FilterByDestination(string destination)
        {
            LockManager.StartReading();
            ICollection<Ride> filteredRides = new List<Ride>();
            filteredRides = MemoryDatabase.GetInstance().Rides
                .Where(ride => ride.EndingLocation.Equals(destination))
                .ToList();
            LockManager.StopReading();
            return filteredRides;
        }

        public ICollection<Review> GetDriverReviews(Guid ride)
        {
            LockManager.StartReading();
            Ride rideToGetReviews = GetRideById(ride);
            User user = _userRepository.GetUserById(rideToGetReviews.DriverId);
            LockManager.StopReading();
            return user.DriverAspects.Reviews;
        }

        public void UpdateRide(Ride rideWithUpdates)
        {
            LockManager.StartWriting();
            Ride rideToUpdate = GetRideById(rideWithUpdates.Id);
            rideToUpdate.AvailableSeats = rideWithUpdates.AvailableSeats;
            rideToUpdate.DepartureTime = rideWithUpdates.DepartureTime;
            rideToUpdate.EndingLocation = rideWithUpdates.EndingLocation;
            rideToUpdate.InitialLocation = rideWithUpdates.InitialLocation;
            rideToUpdate.PricePerPerson = rideWithUpdates.PricePerPerson;
            rideToUpdate.PetsAllowed = rideWithUpdates.PetsAllowed;
            rideToUpdate.VehicleId = rideWithUpdates.VehicleId;
            LockManager.StopWriting();
        }
    }
}