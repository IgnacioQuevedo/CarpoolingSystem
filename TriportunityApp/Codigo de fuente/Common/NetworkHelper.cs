using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Common
{
    public static class NetworkHelper
    {
        private static readonly SettingsManager settingsManager = new SettingsManager();

        public static async Task<TcpClient> ConnectWithServerAsync()
        {
            IPEndPoint local = new IPEndPoint(
                IPAddress.Parse(settingsManager.ReadSettings(ClientConfig.LocalIp)),
                int.Parse(settingsManager.ReadSettings(ClientConfig.LocalPort))
            );

            IPEndPoint server = new IPEndPoint(
                IPAddress.Parse(settingsManager.ReadSettings(ClientConfig.RemoteIp)),
                int.Parse(settingsManager.ReadSettings(ClientConfig.RemotePort))
            );

            TcpClient client = new TcpClient(local);
            await client.ConnectAsync(server);
            return client;
        }

        public static async Task<bool> IsClientConnectedAsync(TcpClient client, CancellationToken token)
        {
            try
            {
                string messageReceived = await ReceiveMessageAsync(client, token);
                Console.WriteLine(messageReceived);

                if (messageReceived.Length > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void CloseTcpConnection(TcpClient client)
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

        private static byte[] EncodeMsgIntoBytes(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }

        private static string DecodeMsgFromBytes(byte[] buffer)
        {
            return Encoding.UTF8.GetString(buffer);
        }

        public static async Task SendMessageAsync(TcpClient client, string message, CancellationToken token)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                byte[] buffer = Encoding.UTF8.GetBytes(message);
                byte[] bufferLength = BitConverter.GetBytes(buffer.Length);

                await stream.WriteAsync(bufferLength, 0, bufferLength.Length, token);

                int totalSent = 0;
                while (totalSent < buffer.Length)
                {
                    int toSend = buffer.Length - totalSent;
                    await stream.WriteAsync(buffer, totalSent, toSend, token);
                    totalSent += toSend;
                }
            }
            catch (IOException ex) when (ex.InnerException is SocketException)
            {
                throw new OperationCanceledException("Connection has been turned off", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while sending message", ex);
            }
        }



        public static async Task<string> ReceiveMessageAsync(TcpClient clientReceiver, CancellationToken token)
        {
            try
            {
                NetworkStream stream = clientReceiver.GetStream();

                byte[] bufferConstantLength = BitConverter.GetBytes(ProtocolConstants.DataLengthSize);

                byte[] msgLengthBuffer = await ReceiveAsync(stream, bufferConstantLength, token);
                byte[] dataBuffer = await ReceiveAsync(stream, msgLengthBuffer, token);

                return DecodeMsgFromBytes(dataBuffer);
            }
            catch (IOException ex) when (ex.InnerException is SocketException)
            {
                throw new OperationCanceledException("Connection has been turn off", ex);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }


        public static async Task<byte[]> ReceiveAsync(NetworkStream clientNetworkStream,
            byte[] bufferWithTheLengthNumber, CancellationToken token)
        {
            int length = BitConverter.ToInt32(bufferWithTheLengthNumber, 0);
            byte[] responseBuffer = new byte[length];

            int size = responseBuffer.Length;
            int offSet = 0;

            while (offSet < size)
            {
                int amountByteSent = await clientNetworkStream.ReadAsync(responseBuffer, offSet, size - offSet, token);

                if (amountByteSent == 0) throw new Exception();

                offSet += amountByteSent;
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

        public static async Task SendImageAsync(TcpClient client, string filePath, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                NetworkStream stream = client.GetStream();

                FileInfo fileInfo = new FileInfo(filePath);
                long fileLength = fileInfo.Length;
                byte[] fileLengthInBytes = BitConverter.GetBytes(fileLength);

                await SendMessageAsync(client, fileInfo.Name, token);
                await stream.WriteAsync(fileLengthInBytes, 0, fileLengthInBytes.Length, token);


                long amountOfParts = ProtocolConstants.AmountOfParts(fileLength);

                token.ThrowIfCancellationRequested();
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    int offset = 0;
                    for (int currentPart = 1; currentPart <= amountOfParts; currentPart++)
                    {
                        bool isLastPart = (currentPart == amountOfParts);
                        int byteAmountToSend = isLastPart ? (int)(fileLength - offset) : ProtocolConstants.MaxPartSize;
                        byte[] buffer = new byte[byteAmountToSend];

                        int readBytes = await fileStream.ReadAsync(buffer, 0, byteAmountToSend, token);

                        await stream.WriteAsync(buffer, 0, readBytes, token);

                        offset += readBytes;
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Operation cancelled, deleting the remaining parts of the file");
                string fileName = Path.GetFileName(filePath);
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName)))
                {
                    File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName));
                }

                throw new OperationCanceledException("Connection has been turn off", e);
            }
            catch (IOException ex) when (ex.InnerException is SocketException)
            {
                throw new OperationCanceledException("Connection has been turn off", ex);
            }
            catch (Exception exceptionCaught)
            {
                throw new Exception(exceptionCaught.Message);
            }
        }

        public static async Task<string> ReceiveImageAsync(TcpClient client, CancellationToken token)
        {

            string fileName = "";
            byte[] bufferFileConstantLength = BitConverter.GetBytes(8);
            try
            {
                string pathDirectoryImageAllocated = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                if (!Directory.Exists(pathDirectoryImageAllocated))
                {
                    Directory.CreateDirectory(pathDirectoryImageAllocated);
                }

                NetworkStream stream = client.GetStream();
                fileName = await ReceiveMessageAsync(client, token);
                byte[] bufferFileLength = await ReceiveAsync(stream, bufferFileConstantLength, token);
                string destinationFilePath = Path.Combine(pathDirectoryImageAllocated, fileName);

                long fileLength = BitConverter.ToInt64(bufferFileLength, 0);
                long amountOfParts = ProtocolConstants.AmountOfParts(fileLength);

                token.ThrowIfCancellationRequested();
                using (FileStream fileNetworkStream =
                       new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write))
                {
                    int offset = 0;
                    for (int currentPart = 1; currentPart <= amountOfParts; currentPart++)
                    {
                        bool isLastPart = (currentPart == amountOfParts);
                        int byteAmountToReceive =
                            isLastPart ? (int)(fileLength - offset) : ProtocolConstants.MaxPartSize;

                        byte[] byteAmountToReceiveInBytes = BitConverter.GetBytes(byteAmountToReceive);
                        token.ThrowIfCancellationRequested();
                        Console.WriteLine($"Receiving part #{currentPart}, of {byteAmountToReceive} bytes");
                        byte[] buffer = await ReceiveAsync(stream, byteAmountToReceiveInBytes, token);
                        await fileNetworkStream.WriteAsync(buffer, 0, buffer.Length, token);

                        offset += buffer.Length;
                    }
                }

                Console.WriteLine(
                    $"Completed sending {fileName}, of  {fileLength} length in bytes, allocated en {destinationFilePath}");

                return destinationFilePath;
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Operation cancelled, deleting the remaining parts of the file");
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName)))
                {
                    File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName));
                }

                throw new OperationCanceledException("Connection has been turned off", e);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public static void CheckIfExceptionIsOperationCanceled(Exception ex)
        {
            if (ex is OperationCanceledException ||
                ex.InnerException is OperationCanceledException || ex.InnerException is SocketException)
            {
                throw new OperationCanceledException();
            }
        }
    }
}