using System.Net.Sockets;

namespace MainServer.Controllers
{
    public class ExceptionsController
    {
        private static Socket _clientSocket;

        public ExceptionsController(Socket clientSocket)
        {
            _clientSocket = clientSocket;
        }
    }
}