using System;
using System.CodeDom;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Common
{
    public static class NetworkHelper
    {
        private static readonly SettingsManager settingsManager = new SettingsManager();
        public static Socket ConnectWithServer()
        {

            IPEndPoint local = new IPEndPoint(
                IPAddress.Parse(settingsManager.ReadSettings(ClientConfig.LocalIp)), int.Parse(settingsManager.ReadSettings(ClientConfig.LocalPort))
            );

            IPEndPoint server = new IPEndPoint(
                IPAddress.Parse(settingsManager.ReadSettings(ClientConfig.RemoteIp)),
                int.Parse(settingsManager.ReadSettings(ClientConfig.RemotePort))
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

        public static void CloseTcpConnections(TcpClient client)
        {
            client.Close();
        }

        public static TcpListener DeployServerListener()
        {
            var localEndPoint = new IPEndPoint(
                IPAddress.Parse(settingsManager.ReadSettings(ServerConfig.LocalIp)),
                int.Parse(settingsManager.ReadSettings(ServerConfig.LocalPort))
            );

            TcpListener serverListener = new TcpListener(localEndPoint);

            Console.WriteLine("Waiting for clients...");
            return serverListener;
        }

        public static byte[] EncodeMsgIntoBytes(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }

        public static string DecodeMsgFromBytes(byte[] buffer)
        {
            return Encoding.UTF8.GetString(buffer);
        }

        public static void SendMessage(TcpClient client, string message)
        {
            Stream stream = client.GetStream();

            byte[] buffer = EncodeMsgIntoBytes(message);

            int size = buffer.Length;
            int offSet = 0;

            stream.Write(buffer, offSet, size);
        }

        public static string ReceiveMessage(TcpClient clientReciever)
        {
            Stream stream = clientReciever.GetStream();

            byte[] bufferConstantLength = BitConverter.GetBytes(ProtocolConstants.DataLengthSize);

            byte[] msgLengthBuffer = Receive(stream, bufferConstantLength);
            byte[] dataBuffer = Receive(stream, msgLengthBuffer);
            return DecodeMsgFromBytes(dataBuffer);
        }


        public static byte[] Receive(Stream clientStream, byte[] bufferWithTheLengthNumber)
        {
            int length = BitConverter.ToInt32(bufferWithTheLengthNumber, 0);
            byte[] responseBuffer = new byte[length];

            int size = responseBuffer.Length;
            int offSet = 0;
            int amountByteSent = 0;

            while (offSet < size)
            {
                amountByteSent =
                    clientStream.Read(responseBuffer, offSet, size - offSet);

                if (amountByteSent == 0) throw new SocketException();

                offSet = offSet + amountByteSent;
            }

            return responseBuffer;
        }

        public static void FilePathValidator(string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                {
                    throw new Exception("The specific file does not exit.");
                }

                if (string.IsNullOrEmpty(filePath)) throw new Exception("The path cannot be empty");
            }
            catch (Exception exceptionCaught)
            {
                throw new Exception(exceptionCaught.Message);
            }
        }

        public static void SendImage(Socket socket, string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
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
            }
            catch (Exception exceptionCaught)
            {
                throw new Exception(exceptionCaught.Message);
            }
        }

        public static string ReceiveImage(Socket socket)
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
                string fileName =
                    Encoding.UTF8.GetString(fileNameInBytes, 0, BitConverter.ToInt32(bufferFileNameLength, 0));
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
                        Console.WriteLine($"Receiving part #{currentPart}, of {byteAmountToReceive} bytes");
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
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}