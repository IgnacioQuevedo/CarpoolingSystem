using System;
using System.CodeDom;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
    public static class NetworkHelper
    {
        public static Socket ConnectWithServer()
        {
            IPEndPoint local = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 0
            );

            IPEndPoint server = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 5000
            );

            Socket newClientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            newClientSocket.Bind(local);
            newClientSocket.Connect(server);
            return newClientSocket;
        }

        public static bool IsSocketConnected(Socket clientSocket)
        {
            try
            {
                string messageReceived = ReceiveMessage(clientSocket);
                Console.WriteLine(messageReceived);

                if (messageReceived.Length > 0)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static void CloseSocketConnections(Socket clientSocket)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        public static Socket DeployServerSocket()
        {
            var localEndPoint = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 5000
            );

            var serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            serverSocket.Bind(localEndPoint);
            serverSocket.Listen(ProtocolConstants.MaxUsersInBackLog);
            Console.WriteLine("Waiting for clients...");
            return serverSocket;
        }

        public static byte[] EncodeMsgIntoBytes(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }

        public static string DecodeMsgFromBytes(byte[] buffer)
        {
            return Encoding.UTF8.GetString(buffer);
        }

        public static void SendMessage(Socket socket, string message)
        {
            Send(socket, BitConverter.GetBytes(message.Length));
            Send(socket, EncodeMsgIntoBytes(message));
        }

        public static string ReceiveMessage(Socket socket)
        {
            //This buffer has the constant length represented in bytes.
            
            byte[] bufferConstantLength = BitConverter.GetBytes(ProtocolConstants.DataLengthSize);

            byte[] msgLengthBuffer = Receive(socket, bufferConstantLength);
            byte[] dataBuffer = Receive(socket, msgLengthBuffer);
            return DecodeMsgFromBytes(dataBuffer);
        }

        public static void Send(Socket clientSocketServerSide, byte[] buffer)
        {
            int size = buffer.Length;
            int offSet = 0;
            int amountByteSent = 0;

            while (size > 0)
            {
                amountByteSent = clientSocketServerSide.Send(buffer, offSet, size, SocketFlags.None);

                if (amountByteSent == 0) throw new SocketException();

                size = size - amountByteSent;
                offSet = offSet + amountByteSent;
            }
        }

        public static byte[] Receive(Socket clientSocketServerSide, byte[] bufferWithTheLengthNumber)
        {
            int length = BitConverter.ToInt32(bufferWithTheLengthNumber, 0);
            byte[] responseBuffer = new byte[length];

            int size = responseBuffer.Length;
            int offSet = 0;
            int amountByteSent = 0;

            while (offSet < size)
            {
                amountByteSent =
                    clientSocketServerSide.Receive(responseBuffer, offSet, size - offSet, SocketFlags.None);

                if (amountByteSent == 0) throw new SocketException();

                offSet = offSet + amountByteSent;
            }

            return responseBuffer;
        }

        public static void SendImageFromClient(Socket socket, string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                {
                    Console.WriteLine("The specific file does not exit.");
                    return;
                }

                SendMessage(socket, fileInfo.Name);

                long fileLength = fileInfo.Length;
                byte[] fileLengthInBytes = BitConverter.GetBytes(fileLength);
                Send(socket, fileLengthInBytes);

                long amountOfParts = ProtocolConstants.AmountOfParts(fileLength);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    int offset = 0;
                    for (int currentPart = 1; currentPart <= amountOfParts; currentPart++)
                    {
                        bool isLastPart = (currentPart == amountOfParts);
                        int byteAmountToSend = isLastPart ? (int)(fileLength - offset) : ProtocolConstants.MaxPartSize;
                        byte[] buffer = new byte[byteAmountToSend];
                        int readBytes = fileStream.Read(buffer, 0, byteAmountToSend);
                        Send(socket, buffer);

                        offset += readBytes;
                    }
                }

                Console.WriteLine(
                    $"Termin� de enviar archivo {fileInfo.Name}, de tama�o {fileLength} bytes, desde {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante la transmisi�n del archivo: {ex.Message}");
            }
        }

        public static string ReceiveImageToServer(Socket socket)
        {
            try
            {
                string pathDirectoryImageAllocated = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                if (!Directory.Exists(pathDirectoryImageAllocated))
                {
                    Directory.CreateDirectory(pathDirectoryImageAllocated);
                }
                
                byte[] bufferConstantNumberLength = BitConverter.GetBytes(ProtocolConstants.DataLengthSize);
                byte[] bufferFileNameLength =
                    Receive(socket, bufferConstantNumberLength); 
                
                byte[] fileNameInBytes = Receive(socket, bufferFileNameLength);
                string fileName = Encoding.UTF8.GetString(fileNameInBytes, 0, BitConverter.ToInt32(bufferFileNameLength,0));
                string destinationFilePath = Path.Combine(pathDirectoryImageAllocated, fileName);


                byte[] bufferFileConstantLength = BitConverter.GetBytes(8);
                byte[] bufferFileLength = Receive(socket, bufferFileConstantLength);

                long fileLength = BitConverter.ToInt64(bufferFileLength, 0);
                long amountOfParts = ProtocolConstants.AmountOfParts(fileLength);

                using (FileStream fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write))
                {
                    int offset = 0;
                    for (int currentPart = 1; currentPart <= amountOfParts; currentPart++)
                    {
                        bool isLastPart = (currentPart == amountOfParts);
                        int byteAmountToReceive =
                            isLastPart ? (int)(fileLength - offset) : ProtocolConstants.MaxPartSize;

                        byte[] byteAmountToReceiveInBytes = BitConverter.GetBytes(byteAmountToReceive);
                        Console.WriteLine($"Recibiendo parte #{currentPart}, de {byteAmountToReceive} bytes");
                        byte[] buffer = Receive(socket, byteAmountToReceiveInBytes);
                        fileStream.Write(buffer, 0, buffer.Length);

                        offset += buffer.Length;
                    }
                }

                Console.WriteLine(
                    $"Completed sending {fileName}, of  {fileLength} length in bytes, allocated en {destinationFilePath}");

                return destinationFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while receiving file:");
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}