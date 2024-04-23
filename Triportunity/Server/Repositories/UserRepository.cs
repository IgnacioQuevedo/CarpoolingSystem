using Server.DataContext;
using System.Linq;
using Server.Objects.Domain.UserModels;
using Server.Objects.Domain;
using Server.Exceptions;
using System;
using System.IO;
using Server.Objects.Domain.ClientModels;
using System.Collections.Generic;
using Server.Objects.Domain.VehicleModels;

namespace Server.Repositories
{
    public static class UserRepository
    {

        public static void RegisterUser(User clientToRegister)
        {
            LockManager.StartWriting();
            if (!UsernameRegistered(clientToRegister.Username))
            {
                MemoryDatabase.GetInstance().Users.Add(clientToRegister);
            }
            else
            {
                throw new UserException("Username already registered");
            }
            LockManager.StopWriting();
        }

        private static bool UsernameRegistered(string username)
        {
            var clientWithThatUsername = FindUserByUsername(username);

            if (clientWithThatUsername != null)
            {
                return false;
            }

            return true;
        }

        public static bool Login(string username, string password)
        {
            LockManager.StartReading();
            var possibleLogin = FindUserByUsername(username);

            if (possibleLogin.Password.Equals(password))
            {
                return true;
            }
            LockManager.StopReading();
            return false;
        }

        public static User FindUserByUsername(string usernameOfClient)
        {
            var clientFound = MemoryDatabase.GetInstance().Users
                .FirstOrDefault(x => x.Username.Equals(usernameOfClient));

            if (clientFound == null)
            {
                throw new UserException("User not found");
            }

            return clientFound;
        }

        public static User FindUserById(Guid id)
        {
            var clientFound = MemoryDatabase.GetInstance().Users
                .FirstOrDefault(x => x.Id.Equals(id));

            if (clientFound == null)
            {
                throw new UserException("User not found");
            }

            return clientFound;
        }

        public static void RegisterDriver(string userName, DriverInfo driveInfo)
        {
            LockManager.StartWriting();
            User user = FindUserByUsername(userName);
            if (user.DriverAspects != null)
            {
                throw new UserException("User is already a driver");
            }
            user.DriverAspects = driveInfo;
            LockManager.StopWriting();
        }

        public static void RateDriver(Guid id, Review review)
        {
            LockManager.StartWriting();
            User user = FindUserById(id);
            if (user.DriverAspects == null)
            {
                throw new UserException("User is not a driver");
            }

            user.DriverAspects.Reviews.Add(review);
            user.DriverAspects.Puntuation = user.DriverAspects.Reviews.Average(x => x.Punctuation);

            LockManager.StopWriting();
        }

        public static void SetVehicle(Guid id, Vehicle vehicle)
        {
            LockManager.StartWriting();
            User user = FindUserById(id);
            if (user.DriverAspects == null)
            {
                throw new UserException("User is not a driver");
            }

            user.DriverAspects.Vehicles.Add(vehicle);
            LockManager.StopWriting();
        }

    }
}