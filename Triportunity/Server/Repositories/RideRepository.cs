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
            Ride rideToJoin = GetRideById(rideId);

            ValidateJoinRide(userId, rideToJoin);

            LockManager.StartWriting();

            rideToJoin.Passengers.Add(userId);
            rideToJoin.AvailableSeats--;

            LockManager.StopWriting();
        }

        private void ValidateJoinRide(Guid user, Ride rideToJoin)
        {
            string exceptionMessage = "";

            LockManager.StartReading();
            if (rideToJoin.AvailableSeats < 1) exceptionMessage = "No are no available seats";
            if (rideToJoin.Passengers.Contains(user) || rideToJoin.DriverId == user) exceptionMessage = "You are already in the ride";
            if (rideToJoin.DepartureTime <= DateTime.Now)
                exceptionMessage = "Cannot join a ride that has already departed";
            LockManager.StopReading();

            if (exceptionMessage != "") throw new RideException(exceptionMessage);
        }

        public Ride GetRideById(Guid rideId)
        {
            LockManager.StartReading();

            var rideToFind = MemoryDatabase.GetInstance().Rides.FirstOrDefault(ride => ride.Id == rideId);

            LockManager.StopReading();

            if (rideToFind == null)
            {
                throw new RideException("Ride not found");
            }

            return rideToFind;
        }

        public void QuitRide(Guid userId, Guid rideId)
        {
            Ride rideToQuit = GetRideById(rideId);

            LockManager.StartReading();

            if (rideToQuit.DriverId.Equals(userId)) {
                throw new RideException("You are the driver, you must disable or delete the ride in order to quit");
            }

            User user = _userRepository.GetUserById(userId);

            LockManager.StopReading();

            if (rideToQuit.DepartureTime <= DateTime.Now)
            {
                throw new RideException("Cannot quit the ride as the departure time has passed.");
            }

            LockManager.StartWriting();

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
                if (ride.Published && ride.DepartureTime > DateTime.Now && ride.AvailableSeats > 0)
                {
                    availableRides.Add(ride);
                }
            }

            if (availableRides.Count == 0)
            {
                throw new RideException("No rides found");
            }

            return availableRides;
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
            ICollection<Ride> filteredRides = new List<Ride>();
            var rides = GetRides();

            LockManager.StartReading();

            filteredRides = rides
                .Where(ride => ride.PricePerPerson >= minPrice && ride.PricePerPerson <= maxPrice)
                .ToList();

            LockManager.StopReading();

            if (filteredRides.Count == 0)
            {
                throw new RideException("No rides found in the selected range");
            }

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

        public void UpdateRide(Ride rideWithUpdates)
        {
            Ride rideToUpdate = GetRideById(rideWithUpdates.Id);
            LockManager.StartWriting();
            rideToUpdate.AvailableSeats = rideWithUpdates.AvailableSeats;
            rideToUpdate.DepartureTime = rideWithUpdates.DepartureTime;
            rideToUpdate.EndingLocation = rideWithUpdates.EndingLocation;
            rideToUpdate.InitialLocation = rideWithUpdates.InitialLocation;
            rideToUpdate.PricePerPerson = rideWithUpdates.PricePerPerson;
            rideToUpdate.PetsAllowed = rideWithUpdates.PetsAllowed;
            rideToUpdate.VehicleId = rideWithUpdates.VehicleId;
            LockManager.StopWriting();
        }

        public void AddReview(Guid rideId, Review review)
        {
            Ride ride = GetRideById(rideId);
            User driver = _userRepository.GetUserById(ride.DriverId);
            LockManager.StartWriting();
            driver.DriverAspects.Reviews.Add(review);
            LockManager.StopWriting();
        }

        public ICollection<Review> GetDriverReviews(Guid ride)
        {
            LockManager.StartReading();
            Ride rideToGetReviews = GetRideById(ride);
            User user = _userRepository.GetUserById(rideToGetReviews.DriverId);
            LockManager.StopReading();
            return user.DriverAspects.Reviews;
        }

        public ICollection<Ride> GetRidesByUser(Guid userId)
        {
            LockManager.StartReading();
            ICollection<Ride> rides = new List<Ride>();
            rides = MemoryDatabase.GetInstance().Rides;
                
            LockManager.StopReading();

            ICollection<Ride> userRides = new List<Ride>();

            foreach (var ride in rides)
            {
                if (ride.DriverId == userId || ride.Passengers.Contains(userId))
                {
                    userRides.Add(ride);
                }
            }



            if (userRides.Count == 0)
            {
                throw new RideException("No rides found");
            }

            return userRides;
        }
    }
}