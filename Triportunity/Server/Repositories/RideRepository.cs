using Server.DataContext;
using Server.Objects.Domain;
using System.Collections.Generic;
using System;
using Server.Objects.DTOs.RideModelDtos;
using System.Linq;
using Server.Exceptions;
using Server.Objects.Domain.UserModels;

namespace Server.Repositories
{
    public class RideRepository
    {

        public static void CreateRide(Ride rideToAdd)
        {
            LockManager.StartWriting();
            rideToAdd.Id = Guid.NewGuid();
            MemoryDatabase.GetInstance().Rides.Add(rideToAdd);
            LockManager.StopWriting();
        }

        public static void JoinRide(Guid userId, Guid rideId)
        {
            LockManager.StartWriting();
            Ride rideToJoin = GetRideById(rideId);
            var user = UserRepository.FindUserById(userId);
            if (rideToJoin.AvailableSeats < 1)
            {
                throw new RideException("No are no available seats");
            }
            if (rideToJoin.Passengers.Contains(user))
            {
                throw new RideException("User is already in the ride");
            }

            rideToJoin.Passengers.Add(user);
            rideToJoin.AvailableSeats--;
            LockManager.StopWriting();
        }

        private static Ride GetRideById(Guid rideId)
        {
            var rideToFind = MemoryDatabase.GetInstance().Rides.FirstOrDefault(x => x.Id == rideId);

            if (rideToFind == null)
            {
                throw new Exception("Ride not found");
            }
            return rideToFind;
        }

        public static void QuitRide(Guid userId, Guid rideId)
        {
            LockManager.StartWriting();
            Ride rideToQuit = GetRideById(rideId);
            var user  = UserRepository.FindUserById(userId);
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

        public static ICollection<Ride> GetRides()
        {

            LockManager.StartReading();
            ICollection<Ride> rides = MemoryDatabase.GetInstance().Rides;
            if (rides == null)
            {
                throw new RideException("No rides found");
            }
            LockManager.StopReading();
            return rides;
        }

        public static void DeleteRide(Guid rideId)
        {
            LockManager.StartWriting();
            Ride rideToDelete = GetRideById(rideId);
            if (rideToDelete.Published)
            {
                throw new RideException("Cannot delete a published ride");
            }
            MemoryDatabase.GetInstance().Rides.Remove(rideToDelete);
            LockManager.StopWriting();
        }

        public static void CancelPublishedRide(Guid rideId)
        {
            LockManager.StartWriting();
            Ride rideToCancel = GetRideById(rideId);
            if (!rideToCancel.Published)
            {
                throw new RideException("Cannot cancel a non-published ride");
            }
            if (rideToCancel.DepartureTime <= DateTime.Now)
            {
                throw new RideException("Cannot cancel a ride that has already departed");
            }
            rideToCancel.Published = false;
            LockManager.StopWriting();
        }

        public ICollection<Ride> FilterByPrice(int minPrice, int maxPrice)
        {
            ICollection<Ride> filteredRides;

            var rides = GetRides();

            LockManager.StartReading();
            filteredRides = rides
                .Where(ride => ride.PricePerPerson >= minPrice && ride.PricePerPerson <= maxPrice)
                .ToList();

            LockManager.StopReading();
            return filteredRides;
        }

        public ICollection<Ride> FilterByInitialLocation(string initialLocation)
        {
            ICollection<Ride> filteredRides;
            LockManager.StartReading();

            filteredRides = MemoryDatabase.GetInstance().Rides
                .Where(ride => ride.InitialLocation.Equals(initialLocation))
                .ToList();
            LockManager.StopReading();
            return filteredRides;
        }

        public ICollection<Ride> FilterByDestination(string destination)
        {
            ICollection<Ride> filteredRides;
            LockManager.StartReading();
            filteredRides = MemoryDatabase.GetInstance().Rides
                .Where(ride => ride.EndingLocation.Equals(destination))
                .ToList();
            LockManager.StopReading();
            return filteredRides;
        }

        public ICollection<Review> GetDriverReviews(Guid ride)
        {
            ICollection<Review> reviews;

            LockManager.StartReading();

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
            LockManager.StopWriting();
        }

    }

}