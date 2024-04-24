using Server.DataContext;
using System.Linq;
using Server.Objects.Domain.UserModels;
using Server.Objects.Domain;
using Server.Exceptions;
using System;
using Server.Objects.Domain.ClientModels;
using Server.Objects.Domain.VehicleModels;

namespace Server.Repositories
{
    public class UserRepository
    {
        public void RegisterUser(User userToRegister)
        {
            LockManager.StartWriting();
            UserAlreadyExists(userToRegister.Username);
            MemoryDatabase.GetInstance().Users.Add(userToRegister);
            LockManager.StopWriting();
        }
        
        private void UserAlreadyExists(string usernameToValidate)
        {
            if (MemoryDatabase.GetInstance().Users.Any(x => x.Username.Equals(usernameToValidate)))
            {
                throw new UserException("User already exists");
            }
        }

        public bool Login(string username, string password)
        {
            LockManager.StartReading();
            User possibleLogin = GetUserByUsername(username);

            if (possibleLogin.Password.Equals(password))
            {
                return true;
            }

            LockManager.StopReading();
            return false;
        }

        public User GetUserByUsername(string usernameOfClient)
        {
            User clientFound = MemoryDatabase.GetInstance().Users
                .FirstOrDefault(x => x.Username.Equals(usernameOfClient));

            if (clientFound == null)
            {
                throw new UserException("User not found");
            }

            return clientFound;
        }

        public User GetUserById(Guid id)
        {
            var clientFound = MemoryDatabase.GetInstance().Users
                .FirstOrDefault(x => x.Id.Equals(id));

            if (clientFound == null)
            {
                throw new UserException("User not found");
            }

            return clientFound;
        }

        public void RegisterDriver(string userName, DriverInfo driveInfo)
        {
            LockManager.StartWriting();
            User userFound = GetUserByUsername(userName);
            if (userFound.DriverAspects != null)
            {
                throw new UserException("User is already a driver");
            }

            userFound.DriverAspects = driveInfo;
            LockManager.StopWriting();
        }

        public void RateDriver(Guid id, Review review)
        {
            LockManager.StartWriting();
            User user = GetUserById(id);
            if (user.DriverAspects == null)
            {
                throw new UserException("User is not a driver");
            }

            user.DriverAspects.Reviews.Add(review);
            user.DriverAspects.Puntuation = user.DriverAspects.Reviews.Average(x => x.Punctuation);

            LockManager.StopWriting();
        }

        public void SetVehicle(Guid id, Vehicle vehicle)
        {
            LockManager.StartWriting();
            User user = GetUserById(id);
            if (user.DriverAspects == null)
            {
                throw new UserException("User is not a driver");
            }

            user.DriverAspects.Vehicles.Add(vehicle);
            LockManager.StopWriting();
        }
        
    }
}