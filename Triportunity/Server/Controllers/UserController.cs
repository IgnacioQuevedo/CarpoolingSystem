using System;
using System.Collections.Generic;
using System.Linq;
using Server.Exceptions;
using Server.Objects.Domain.ClientModels;
using Server.Objects.Domain.UserModels;
using Server.Objects.Domain.VehicleModels;
using Server.Objects.DTOs.ClientModelDtos;
using Server.Objects.DTOs.ReviewModelDtos;
using Server.Objects.DTOs.UserModelDtos;
using Server.Objects.DTOs.VehicleModelDto;
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
                _userRepository.RegisterClient(userToRegister);
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

        public UserDto GetUserById(Guid userId)
        {
            try
            {
                User userInDb = _userRepository.UserById(userId);
                List<ReviewDto> reviewsDto = null;
                List<VehicleDto> vehiclesDto = null;
                
                if (userInDb.DriverAspects != null)
                {
                    reviewsDto = userInDb.DriverAspects.Reviews
                        .Select(review => new ReviewDto(review.Id, review.Punctuation, review.Comment)).ToList();

                    vehiclesDto = userInDb.DriverAspects.Vehicles
                        .Select(vehicle => new VehicleDto(vehicle.Id, vehicle.DestinationFilePath)).ToList();
                }

                UserDto userToReturn = new UserDto(userInDb.Id, userInDb.Username, userInDb.Password,new DriverInfoDto(reviewsDto,vehiclesDto));
                return userToReturn;
            }
            catch (UserException exceptionCaught)
            {
                throw new Exception(exceptionCaught.Message);
            }
        }
    }
}