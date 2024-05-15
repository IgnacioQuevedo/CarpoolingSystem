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
        public static TcpClient ConnectWithServer()
        {

            IPEndPoint local = new IPEndPoint(
                IPAddress.Parse(settingsManager.ReadSettings(ClientConfig.LocalIp)), int.Parse(settingsManager.ReadSettings(ClientConfig.LocalPort))
            );

            IPEndPoint server = new IPEndPoint(
                IPAddress.Parse(settingsManager.ReadSettings(ClientConfig.RemoteIp)),
                int.Parse(settingsManager.ReadSettings(ClientConfig.RemotePort))
            );

            TcpClient client = new TcpClient(local);
            client.Connect(server);

            return client;
        }

        public static bool IsClientConnected(TcpClient client)
        {
            try
            {
                string messageReceived = ReceiveMessage(client);
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
            NetworkStream NetworkStream = client.GetStream();

            byte[] bufferLength = BitConverter.GetBytes(message.Length);
            NetworkStream.Write(bufferLength, 0, ProtocolConstants.DataLengthSize);

            byte[] buffer = EncodeMsgIntoBytes(message);

            int size = buffer.Length;
            int offSet = 0;

            NetworkStream.Write(buffer, offSet, size);
        }

        public static string ReceiveMessage(TcpClient clientReciever)
        {
            NetworkStream NetworkStream = clientReciever.GetStream();

            byte[] bufferConstantLength = BitConverter.GetBytes(ProtocolConstants.DataLengthSize);

            byte[] msgLengthBuffer = Receive(NetworkStream, bufferConstantLength);
            byte[] dataBuffer = Receive(NetworkStream, msgLengthBuffer);
            return DecodeMsgFromBytes(dataBuffer);
        }


        public static byte[] Receive(NetworkStream clientNetworkStream, byte[] bufferWithTheLengthNumber)
        {
            int length = BitConverter.ToInt32(bufferWithTheLengthNumber, 0);
            byte[] responseBuffer = new byte[length];

            int size = responseBuffer.Length;
            int offSet = 0;
            int amountByteSent = 0;

            while (offSet < size)
            {
                amountByteSent =
                    clientNetworkStream.Read(responseBuffer, offSet, size - offSet);

                if (amountByteSent == 0) throw new Exception();

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

        public static void SendImage(TcpClient client, string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                SendMessage(client, fileInfo.Name);

                NetworkStream NetworkStream = client.GetStream();

                long fileLength = fileInfo.Length;
                byte[] fileLengthInBytes = BitConverter.GetBytes(fileLength);
                NetworkStream.Write(fileLengthInBytes, 0, fileLengthInBytes.Length);

                long amountOfParts = ProtocolConstants.AmountOfParts(fileLength);

                using (FileStream fileNetworkStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    int offset = 0;
                    for (int currentPart = 1; currentPart <= amountOfParts; currentPart++)
                    {
                        bool isLastPart = (currentPart == amountOfParts);
                        int byteAmountToSend = isLastPart ? (int)(fileLength - offset) : ProtocolConstants.MaxPartSize;
                        byte[] buffer = new byte[byteAmountToSend];
                        int readBytes = fileNetworkStream.Read(buffer, 0, byteAmountToSend);
                        
                        NetworkStream.Write(buffer, 0, readBytes);

                        offset += readBytes;
                    }
                }
            }
            catch (Exception exceptionCaught)
            {
                throw new Exception(exceptionCaught.Message);
            }
        }

        public static string ReceiveImage(TcpClient client)
        {
            try
            {
                string pathDirectoryImageAllocated = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                if (!Directory.Exists(pathDirectoryImageAllocated))
                {
                    Directory.CreateDirectory(pathDirectoryImageAllocated);
                }

                NetworkStream stream = client.GetStream();

                byte[] bufferConstantNumberLength = BitConverter.GetBytes(ProtocolConstants.DataLengthSize);
                byte[] bufferFileNameLength =
                    Receive(stream, bufferConstantNumberLength);

                byte[] fileNameInBytes = Receive(stream, bufferFileNameLength);
                string fileName =
                    Encoding.UTF8.GetString(fileNameInBytes, 0, BitConverter.ToInt32(bufferFileNameLength, 0));
                string destinationFilePath = Path.Combine(pathDirectoryImageAllocated, fileName);


                byte[] bufferFileConstantLength = BitConverter.GetBytes(8);
                byte[] bufferFileLength = Receive(stream, bufferFileConstantLength);

                long fileLength = BitConverter.ToInt64(bufferFileLength, 0);
                long amountOfParts = ProtocolConstants.AmountOfParts(fileLength);

                using (FileStream fileNetworkStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write))
                {
                    int offset = 0;
                    for (int currentPart = 1; currentPart <= amountOfParts; currentPart++)
                    {
                        bool isLastPart = (currentPart == amountOfParts);
                        int byteAmountToReceive =
                            isLastPart ? (int)(fileLength - offset) : ProtocolConstants.MaxPartSize;

                        byte[] byteAmountToReceiveInBytes = BitConverter.GetBytes(byteAmountToReceive);
                        Console.WriteLine($"Receiving part #{currentPart}, of {byteAmountToReceive} bytes");
                        byte[] buffer = Receive(stream, byteAmountToReceiveInBytes);
                        fileNetworkStream.Write(buffer, 0, buffer.Length);

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