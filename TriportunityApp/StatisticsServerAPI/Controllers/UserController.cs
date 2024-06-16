using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StatisticsServerAPI.Services;

namespace StatisticsServerAPI.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet]
        [Route("login-events")]
        public IActionResult GetLoginEvents()
        {
            try
            {
                return Ok(_userService.GetLoginEvents());
            }
            catch (Exception)
            {
                ObjectResult objectResult = StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                return objectResult;
            }
        }
    }
}
