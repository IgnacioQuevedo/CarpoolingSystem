using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NuGet.Protocol.Plugins;

namespace AdministrativeServer.Controllers
{
    [ApiController]
    [Route("administrative/rides")]
    public class RidesController : Controller
    {
        private HandlerOfRequests _handlerOfRequests; 
        // I Believe we should create a general domain out of this folder on the folder were we have all the servers
        // So that we can share the same domain between the servers
        private readonly IConfiguration _configuration;
        private GrpcChannel _channel;
            
        public RidesController(IConfiguration configuration)
        {
            _configuration = configuration;
            _channel = GrpcChannel.ForAddress(_configuration[AdministrativeConfig.serverIPconfigkey]);
        }

    }
    
}
