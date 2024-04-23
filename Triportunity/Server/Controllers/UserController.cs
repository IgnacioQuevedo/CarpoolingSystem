using System;
using System.Linq;
using Server.Exceptions;
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
                User userToRegister = new User(request.Username, request.Password,request.Ci, null);
                _userRepository.RegisterClient(userToRegister);
            }
            catch (UserException exception)
            {
                throw new Exception(exception.Message);
            }
        }
        
    }
}