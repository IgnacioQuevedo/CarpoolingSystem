using Server.DataContext;
using System.Linq;
using Server.Objects.Domain.UserModels;
using Server.Objects.Domain;
using Server.Exceptions;
using System;
using Server.Objects.Domain.VehicleModels;

namespace Server.Repositories
{
    public class UserRepository
    {
        public void RegisterUser(User userToRegister)
        {
            UserAlreadyExists(userToRegister.Username);

            LockManager.StartWriting();

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

        public User Login(string username, string password)
        {
            User possibleLogin = new User();
            possibleLogin = GetUserByUsername(username);

            if (possibleLogin.Password.Equals(password))
            {
                return possibleLogin;
            }

            throw new UserException("Invalid credentials");
        }

        public User GetUserByUsername(string usernameOfClient)
        {
            LockManager.StartReading();

            User clientFound = MemoryDatabase.GetInstance().Users
                .FirstOrDefault(x => x.Username.Equals(usernameOfClient));

            LockManager.StopReading();

            if (clientFound == null)
            {
                throw new UserException("User not found");
            }
            return clientFound;
        }

        public User GetUserById(Guid id)
        {
            LockManager.StartReading();

            User clientFound = new User();
            clientFound = MemoryDatabase.GetInstance().Users
             .FirstOrDefault(x => x.Id.Equals(id));

            LockManager.StopReading();

            if (clientFound == null)
            {
                throw new UserException("User not found");
            }

            return clientFound;
        }

        public void RegisterDriver(Guid userId, DriverInfo driveInfo)
        {
            User userFound = new User();
            userFound = GetUserById(userId);

            if (userFound.DriverAspects != null)
            {
                throw new UserException("User is already a driver");
            }

            LockManager.StartWriting();
            userFound.DriverAspects = driveInfo;
            LockManager.StopWriting();
        }
        
        public void DeleteDriver(Guid userId)
        {
            
            User userFound = new User();
            userFound = GetUserById(userId);
            
            LockManager.StartWriting();
            userFound.DriverAspects = null;
            LockManager.StopWriting();
        }

        public void RateDriver(Guid id, Review review)
        {
            User user = new User();
            user = GetUserById(id);

            if (user.DriverAspects == null)
            {
                throw new UserException("User is not a driver");
            }

            LockManager.StartWriting();

            user.DriverAspects.Reviews.Add(review);
            user.DriverAspects.Puntuation = user.DriverAspects.Reviews.Average(x => x.Punctuation);

            LockManager.StopWriting();
        }

        public void AddVehicle(Guid id, Vehicle vehicle)
        {
            User user = new User();
            user = GetUserById(id);

            if (user.DriverAspects == null)
            {
                throw new UserException("User is not a driver");
            }

            LockManager.StartWriting();

            user.DriverAspects.Vehicles.Add(vehicle);

            LockManager.StopWriting();
        }

        public Vehicle GetVehicleById(Guid userId, Guid vehicleId)
        {
            User user = new User();
            user = GetUserById(userId);

            if (user.DriverAspects != null)
            {
                LockManager.StartReading();

                Vehicle vehicle = user.DriverAspects.Vehicles.FirstOrDefault(x => x.Id.Equals(vehicleId));

                LockManager.StopReading();

                if (vehicle == null)
                {
                    throw new UserException("Vehicle not found");
                }

                return vehicle;
            }


            throw new UserException("User is not a driver");
        }

     
    }
}