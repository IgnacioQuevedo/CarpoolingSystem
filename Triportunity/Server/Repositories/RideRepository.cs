using Server.DataContext;
using Server.Objects.Domain;
using System.Collections.Generic;
using System;
using Server.Objects.DTOs.RideModelDtos;
using System.Linq;
using Client.Services;
using Server.Exceptions;
using Server.Objects.Domain.UserModels;

namespace Server.Repositories
{
    public class RideRepository
    {
        public static UserRepository _userRepository = new UserRepository();

        public static void CreateRide(Ride rideToAdd)
        {
            LockManager.StartWriting();
            MemoryDatabase.GetInstance().Rides.Add(rideToAdd);
            LockManager.StopWriting();
        }

        public static void JoinRide(Guid userId, Guid rideId)
        {
            LockManager.StartWriting();
            Ride rideToJoin = GetRideById(rideId);
            User user = _userRepository.GetUserById(userId);

            ValidateJoinRide(user, rideToJoin);
            rideToJoin.Passengers.Add(user);
            rideToJoin.AvailableSeats--;
            LockManager.StopWriting();
        }

        private static void ValidateJoinRide(User user, Ride rideToJoin)
        {
            string exceptionMessage = "";

            if (rideToJoin.AvailableSeats < 1) exceptionMessage = "No are no available seats";
            if (rideToJoin.Passengers.Contains(user)) exceptionMessage = "User is already in the ride";
            if (rideToJoin.DepartureTime <= DateTime.Now)
                exceptionMessage = "Cannot join a ride that has already departed";

            if (exceptionMessage != "") throw new RideException(exceptionMessage);
        }

        private static Ride GetRideById(Guid rideId)
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

            if (!rideToQuit.Passengers.Contains(user))
            {
                throw new RideException("User is not in the ride");
            }

            if (rideToQuit.DepartureTime <= DateTime.Now)
            {
                throw new RideException("Cannot quit the ride as the departure time has passed.");
            }

            rideToQuit.Passengers.Remove(user);
            rideToQuit.AvailableSeats++;
            LockManager.StopWriting();
        }

        public ICollection<Ride> GetRides()
        {
            LockManager.StartReading();
            ICollection<Ride> rides = (ICollection<Ride>)MemoryDatabase.GetInstance().Rides.FirstOrDefault(ride => ride.Published == true);
            if (rides == null)
            {
                throw new RideException("No rides found");
            }

            foreach (var ride in rides)
            {
                if (ride.DepartureTime <= DateTime.Now)
                {
                    DeleteRide(ride.Id);
                }
            }

            LockManager.StopReading();
            return rides;
        }

        public void DeleteRide(Guid rideId)
        {
            LockManager.StartWriting();
            Ride rideToDelete = GetRideById(rideId);
            MemoryDatabase.GetInstance().Rides.Remove(rideToDelete);
            LockManager.StopWriting();
        }

        public static void DisablePublishedRide(Guid rideId)
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

        public ICollection<Ride> FilterByInitialLocation(string initialLocation)
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
            ICollection<Review> reviews = new List<Review>();

            Ride actualRide = GetRideById(ride);
            reviews = actualRide.Driver.DriverAspects.Reviews;

            if (reviews == null)
            {
                throw new DriverInfoException("No reviews found");
            }

            LockManager.StopReading();
            return reviews;
        }

        public static void UpdateRide(Ride rideWithUpdates)
        {
            LockManager.StartWriting();
            Ride rideToUpdate = GetRideById(rideWithUpdates.Id);
            rideToUpdate.AvailableSeats = rideWithUpdates.AvailableSeats;
            rideToUpdate.DepartureTime = rideWithUpdates.DepartureTime;
            rideToUpdate.EndingLocation = rideWithUpdates.EndingLocation;
            rideToUpdate.InitialLocation = rideWithUpdates.InitialLocation;
            rideToUpdate.PricePerPerson = rideWithUpdates.PricePerPerson;
            rideToUpdate.PetsAllowed = rideWithUpdates.PetsAllowed;
            rideToUpdate.PhotoPath = rideWithUpdates.PhotoPath;
            LockManager.StopWriting();
        }
    }
}