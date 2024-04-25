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
    
    public static void HandleException(string[] messageArray)
    {
        string message = ProtocolConstants.Exception + ";" + CommandsConstraints.ManageException + ";" + messageArray;
        NetworkHelper.SendMessage(_clientSocket, message);
    }
}