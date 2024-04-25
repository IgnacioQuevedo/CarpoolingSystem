using System.Net.Sockets;
using Common;
using Server.Repositories;

namespace Server.Controllers;

public class ExceptionsController
{
    private static Socket _clientSocket;

    public ExceptionsController(Socket clientSocket)
    {
        _clientSocket = clientSocket;
    }
}