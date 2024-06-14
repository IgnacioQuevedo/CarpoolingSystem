using MainServer.DataContext;
using MainServer.Exceptions;
using MainServer.Objects.Domain;
using MainServer.Objects.Domain.Enums;
using MainServer.Objects.Domain.UserModels;
using MainServer.Objects.Domain.VehicleModels;

namespace MainServer.Repositories
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
            string exceptionMessage = "";

            LockManager.StartReading();

            if (MemoryDatabase.GetInstance().Users.Any(x => x.Username.Equals(usernameToValidate)))
            {
                exceptionMessage = "User already exists";
            }

            LockManager.StopReading();

            if (!exceptionMessage.Equals(""))
            {
                throw new UserException(exceptionMessage);
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
            string exceptionMessage = "";

            LockManager.StartReading();

            User clientFound = MemoryDatabase.GetInstance().Users
                .FirstOrDefault(x => x.Username.Equals(usernameOfClient));

            if (clientFound == null)
            {
                exceptionMessage = "User not found";
            }

            LockManager.StopReading();

            if (!exceptionMessage.Equals(""))
            {
                throw new UserException(exceptionMessage);
            }

            return clientFound;
        }

        public User GetUserById(Guid id)
        {
            string exceptionMessage = "";

            LockManager.StartReading();

            User clientFound = new User();
            clientFound = MemoryDatabase.GetInstance().Users
             .FirstOrDefault(x => x.Id.Equals(id));

            if (clientFound == null)
            {
                exceptionMessage = "User not found";
            }

            LockManager.StopReading();

            if (!exceptionMessage.Equals(""))
            {
                throw new UserException(exceptionMessage);
            }

            return clientFound;
        }

        public void RegisterDriver(Guid userId, DriverInfo driveInfo)
        {
            User userFound = new User();
            string exceptionMessage = "";

            userFound = GetUserById(userId);

            LockManager.StartReading();

            if (userFound.DriverAspects != null)
            {
                exceptionMessage = "User is already a driver";
            }

            LockManager.StopReading();

            if (!exceptionMessage.Equals(""))
            {
                throw new UserException(exceptionMessage);
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
            string exceptionMessage = "";

            user = GetUserById(id);

            LockManager.StartReading();

            if (user.DriverAspects == null)
            {
                exceptionMessage = "User is not a driver";
            }

            LockManager.StopReading();

            if (!exceptionMessage.Equals(""))
            {
                throw new UserException(exceptionMessage);
            }

            LockManager.StartWriting();

            user.DriverAspects.Reviews.Add(review);
            user.DriverAspects.Puntuation = user.DriverAspects.Reviews.Average(x => x.Punctuation);

            LockManager.StopWriting();
        }

        public void AddVehicle(Guid id, Vehicle vehicle)
        {
            User user = new User();
            string exceptionMessage = "";

            user = GetUserById(id);

            LockManager.StartReading();

            if (user.DriverAspects == null)
            {
                exceptionMessage = "User is not a driver";
            }

            LockManager.StopReading();

            if (!exceptionMessage.Equals(""))
            {
                throw new UserException(exceptionMessage);
            }

            LockManager.StartWriting();

            user.DriverAspects.Vehicles.Add(vehicle);

            LockManager.StopWriting();
        }

        public Vehicle GetVehicleById(Guid userId, Guid vehicleId)
        {
            User user = new User();
            Vehicle vehicle = null;
            string exceptionMessage = "";

            user = GetUserById(userId);

            LockManager.StartReading();

            if (user.DriverAspects != null)
            {

                vehicle = user.DriverAspects.Vehicles.FirstOrDefault(x => x.Id.Equals(vehicleId));


                if (vehicle == null)
                {
                    exceptionMessage = "Vehicle not found";
                }
            }
            else
            {
                exceptionMessage = "User is not a driver";
            }

            LockManager.StopReading();

            if (!exceptionMessage.Equals(""))
            {
                throw new UserException(exceptionMessage);
            }

            return vehicle;
        }
    }
}
