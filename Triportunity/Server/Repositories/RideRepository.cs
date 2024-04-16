using Server.DataContext;
using Server.Objects.Domain;
using System.Collections.Generic;
using System;
using System.Diagnostics.Tracing;
using System.Linq;

namespace Server.Repositories
{
    public class RideRepository
    {

        public void CreateRide(Ride rideToAdd)
        {
            LockManager.StartWriting();
            MemoryDatabase.GetInstance().Rides.Add(rideToAdd);
            LockManager.StopWriting();
        }

        public ICollection<Ride> GetRides()
        {

            LockManager.StartReading();
            ICollection<Ride> rides = MemoryDatabase.GetInstance().Rides;
            LockManager.StopReading();
            return rides;
        }


        public void UpdateRide(Ride rideWithUpdates)
        {
            LockManager.StartWriting();
            MemoryDatabase.GetInstance().Rides.Remove(GetRideById(rideWithUpdates.Id));
            CreateRide(rideWithUpdates);
            LockManager.StopWriting();
        }

        private Ride GetRideById(Guid rideId)
        {
            LockManager.StartReading();
            var rideToFind = MemoryDatabase.GetInstance().Rides.FirstOrDefault(x => x.Id == rideId);
            LockManager.StopReading();
            return rideToFind;
        }


        public void DeleteRide(int rideId)
        {
            LockManager.StartWriting();
            MemoryDatabase.GetInstance().Rides.FirstOrDefault(r => r.Id.Equals(rideId));
            LockManager.StopWriting();
        }

        public ICollection<Ride> FilterByPrice(int minPrice, int maxPrice)
        {
            ICollection<Ride> filteredRides;
            LockManager.StartReading();
            filteredRides = MemoryDatabase.GetInstance().Rides
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

    }

}