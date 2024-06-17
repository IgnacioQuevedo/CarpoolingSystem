using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class FileNetworkStreamHelper
    {
        public static async Task<byte[]> ReadFile(string path, long offset, int size)
        {
            byte[] buffer = new byte[size];

            using (var fileNetworkStream = new FileStream(path, FileMode.Open))
            {
                fileNetworkStream.Position = offset;
                int readBytes = 0;
                while (readBytes < size)
                {
                    int bytesRecieved = await fileNetworkStream.ReadAsync(buffer, readBytes, size - readBytes);
                    if (bytesRecieved == 0)
                    {
                        throw new FileNotFoundException();
                    }
                    readBytes += bytesRecieved;
                }
            }
            return buffer;
        }

        public async Task WriteFile(string fileName, byte[] data)
        {
            if (File.Exists(fileName))
            {
                using (var fileNetworkStream = new FileStream(fileName, FileMode.Append))
                {
                    await fileNetworkStream.WriteAsync(data, 0, data.Length);
                }
            }
            else
            {
                using (var fileNetworkStream = new FileStream(fileName, FileMode.Create))
                {
                    await fileNetworkStream.WriteAsync(data, 0, data.Length);
                }
            }
        }

    }
}
