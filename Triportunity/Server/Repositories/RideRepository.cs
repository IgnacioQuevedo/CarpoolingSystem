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
            rideToAdd.DepartureTime = rideToAdd.DepartureTime.ToUniversalTime();
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
            if (rideToJoin.Passengers.Contains(user) || rideToJoin.DriverId == user)
                exceptionMessage = "You are already in the ride";
            if (rideToJoin.DepartureTime <= DateTime.Now)
                exceptionMessage = "Cannot join a ride that has already departed";
            LockManager.StopReading();

            if (exceptionMessage != "") throw new RideException(exceptionMessage);
        }

        public Ride GetRideById(Guid rideId)
        {
            LockManager.StartReading();

            var rideToFind = MemoryDatabase.GetInstance().Rides.FirstOrDefault(ride => ride.Id == rideId);

            string exceptionMessage = "";

            if (rideToFind == null)
            {
                exceptionMessage = "Ride not found";
            }

            LockManager.StopReading();

            if (exceptionMessage != "")
            {
                throw new RideException(exceptionMessage);
            }

            LockManager.StartWriting();
            rideToFind.DepartureTime = rideToFind.DepartureTime.ToLocalTime();
            LockManager.StopWriting();

            return rideToFind;
        }

        public void QuitRide(Guid userId, Guid rideId)
        {
            Ride rideToQuit = GetRideById(rideId);

            string exceptionMessage = "";

            LockManager.StartReading();

            if (rideToQuit.DriverId.Equals(userId))
            {
                exceptionMessage = "Cannot quit the ride as you are the driver.";
            }

            if (rideToQuit.DepartureTime <= DateTime.Now)
            {
                exceptionMessage = "Cannot quit a ride that has already departed";
            }

            LockManager.StopReading();

            if (exceptionMessage != "")
            {
                throw new RideException(exceptionMessage);
            }

            LockManager.StartWriting();
            rideToQuit.Passengers.Remove(userId);
            rideToQuit.AvailableSeats++;
            LockManager.StopWriting();
        }

        public ICollection<Ride> GetRides()
        {
            ICollection<Ride> rides = new List<Ride>();

            string exceptionMessage = "";

            LockManager.StartReading();

            rides = MemoryDatabase.GetInstance().Rides;

            ICollection<Ride> availableRides = new List<Ride>();

            foreach (var ride in rides)
            {
                if (ride.Published && ride.DepartureTime > DateTime.Now && ride.AvailableSeats > 0)
                {
                    ride.DepartureTime = ride.DepartureTime.ToLocalTime();
                    availableRides.Add(ride);
                }
            }

            if (availableRides.Count == 0)
            {
                exceptionMessage = "No rides found";
            }

            LockManager.StopReading();

            if (exceptionMessage != "")
            {
                throw new RideException(exceptionMessage);
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
            Ride rideToCancel = GetRideById(rideId);
            LockManager.StartWriting();
            rideToCancel.Published = false;
            LockManager.StopWriting();
        }


        public ICollection<Ride> FilterByPrice(double minPrice, double maxPrice)
        {
            ICollection<Ride> filteredRides = new List<Ride>();

            string exceptionMessage = "";

            LockManager.StartReading();

            var rides = MemoryDatabase.GetInstance().Rides;

            filteredRides = rides.Where(ride => ride.PricePerPerson >= minPrice && ride.PricePerPerson <= maxPrice)
                .ToList();

            if (filteredRides.Count == 0)
            {
                exceptionMessage = "No rides found";
            }

            LockManager.StopReading();

            if (exceptionMessage != "")
            {
                throw new RideException(exceptionMessage);
            }

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

        public void AddReview(Guid actualUserId, Guid rideId, Review review)
        {
            Ride ride = GetRideById(rideId);
            User driver = _userRepository.GetUserById(ride.DriverId);

            string exceptionMessage = "";

            LockManager.StartReading();

            if (actualUserId.Equals(driver.Id))
            {
                exceptionMessage = "You can't review your own ride";
            }

            if (ride.DepartureTime > DateTime.Now)
            {
                exceptionMessage = "You can't review a ride that hasn't happened yet";
            }

            LockManager.StopReading();

            if (exceptionMessage != "")
            {
                throw new RideException(exceptionMessage);
            }

            LockManager.StartWriting();
            driver.DriverAspects.Reviews.Add(review);
            LockManager.StopWriting();
        }

        public ICollection<Review> GetDriverReviews(Guid ride)
        {
            Ride rideToGetReviews = GetRideById(ride);
            User user = _userRepository.GetUserById(rideToGetReviews.DriverId);
            return user.DriverAspects.Reviews;
        }

        public ICollection<Ride> GetRidesByUser(Guid userId)
        {
            ICollection<Ride> rides = new List<Ride>();

            ICollection<Ride> userRides = new List<Ride>();

            string exceptionMessage = "";

            LockManager.StartReading();

            rides = MemoryDatabase.GetInstance().Rides;

            foreach (var ride in rides)
            {
                if (ride.DriverId == userId || ride.Passengers.Contains(userId))
                {
                    ride.DepartureTime = ride.DepartureTime.ToLocalTime();
                    userRides.Add(ride);
                }
            }

            if (userRides.Count == 0)
            {
                exceptionMessage = "No rides found";
            }

            LockManager.StopReading();

            if (exceptionMessage != "")
            {
                throw new RideException(exceptionMessage);
            }

            return userRides;
        }
    }
}