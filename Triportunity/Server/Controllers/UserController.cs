using System;
using System.Collections.Generic;
using System.Linq;
using Client.Objects.ReviewModels;
using Client.Objects.UserModels;
using Client.Objects.VehicleModels;
using Server.Exceptions;
using Server.Objects.Domain;
using Server.Objects.Domain.ClientModels;
using Server.Objects.Domain.UserModels;
using Server.Objects.Domain.VehicleModels;
using Server.Objects.DTOs.ClientModelDtos;
using Server.Repositories;


namespace Server.Controllers
{
    public class ClientController
    {
        
        private UserRepository _userRepository = new UserRepository();

        public void RegisterUser(RegisterUserRequestDto request)
        {
            try
            {
                User userToRegister = new User(request.Username, request.Password, request.Ci, null);
                _userRepository.RegisterUser(userToRegister);
            }
            catch (UserException exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public bool LoginUser(string username, string password)
        {
            try
            {
                return _userRepository.Login(username, password);
            }
            catch (UserException exceptionCaught)
            {
                throw new Exception(exceptionCaught.Message);
            }
        }

        public UserClient GetUserById(Guid userId)
        {
            try
            {
                User userInDb = _userRepository.GetUserById(userId);
                DriverInfo driverInfoOfUser = userInDb.DriverAspects;

                List<ReviewClient> reviewsClient = null;
                List<VehicleClient> vehiclesClient = null;

                if (userInDb.DriverAspects != null)
                {
                    reviewsClient = userInDb.DriverAspects.Reviews
                        .Select(review => new ReviewClient(review.Id, review.Punctuation, review.Comment)).ToList();

                    vehiclesClient = userInDb.DriverAspects.Vehicles
                        .Select(vehicle => new VehicleClient(vehicle.Id, vehicle.DestinationFilePath)).ToList();
                }

                UserClient userToReturn = new UserClient(userInDb.Id, userInDb.Username, userInDb.Password,
                    new DriverInfoClient(driverInfoOfUser.Puntuation, reviewsClient, vehiclesClient));

                return userToReturn;
            }

            catch (UserException exceptionCaught)
            {
                throw new Exception(exceptionCaught.Message);
            }
        }

        public void RateDriver(Guid driverId, ReviewClient reviewToAdd)
        {
            try
            {
                _userRepository.RateDriver(driverId, new Review(reviewToAdd.Punctuation, reviewToAdd.Comment));
            }
            catch (UserException exceptionCaught)
            {
                throw new Exception(exceptionCaught.Message);
            }
        }
        public void SetVehicle(Guid driverId, VehicleClient vehicleToAdd)
        {
            try
            {
                _userRepository.SetVehicle(driverId, new Vehicle(vehicleToAdd.ImageFileName));
            }
            catch (UserException exceptionCaught)
            {
                throw new Exception(exceptionCaught.Message);
            }
        }
    }
}