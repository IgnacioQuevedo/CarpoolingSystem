using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class FileStreamHelper
    {
        public static byte[] ReadFile(string path, long offset, int size)
        {
            byte[] buffer = new byte[size];

            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                fileStream.Position = offset;
                int readBytes = 0;
                while (readBytes < size)
                {
                    int bytesRecieved = fileStream.Read(buffer, readBytes, size - readBytes);
                    if (bytesRecieved == 0)
                    {
                        throw new FileNotFoundException();
                    }
                    readBytes += bytesRecieved;
                }
            }
            return buffer;
        }

        public void WriteFile(string fileName, byte[] data)
        {
            if (File.Exists(fileName))
            {
                using (var fileStream = new FileStream(fileName, FileMode.Append))
                {
                    fileStream.Write(data, 0, data.Length);
                }
            }
            else
            {
                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    fileStream.Write(data, 0, data.Length);
                }
            }
        }

    }
}
