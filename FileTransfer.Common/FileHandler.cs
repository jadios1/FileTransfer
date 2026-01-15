using System.Net.Sockets;
using System.Text.Json;

namespace FileTransfer.Common;

public class FileHandler
{
       private FileData? _filedata;
       private NetworkStream _networkStream;
       private FileStream _fileStream;

       //Constructor for sending the file
       public FileHandler(string FilePath,NetworkStream networkStream)
       {
              _networkStream = networkStream;
              _fileStream = new FileStream(FilePath,FileMode.Open,FileAccess.Read);
              var info = new FileInfo(FilePath);
              
              _filedata = new FileData
              {
                     FileName = Path.GetFileName(FilePath),
                     FileSize = info.Length,
                     FileType = Path.GetExtension(FilePath)
              };

       }
       
       //Constructor for reading the file
       public FileHandler(NetworkStream networkStream)
       {
              _networkStream = networkStream;
       }
       
       public void FileSend()
       {
              string jsonString = JsonSerializer.Serialize(_filedata);
              byte[] jsonStringBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

              byte [] jsonStringLenBytes = BitConverter.GetBytes(jsonStringBytes.Length);
              
              int jsonLen = jsonStringBytes.Length;
              _networkStream.Write(jsonStringLenBytes, 0, jsonStringLenBytes.Length);
              _networkStream.Write(jsonStringBytes, 0, jsonStringBytes.Length);
              
              byte[] buffer = new byte[8192];
              int bytesRead;
              
              while ((bytesRead = _fileStream.Read(buffer, 0, buffer.Length)) > 0)
              {
                     _networkStream.Write(buffer, 0, bytesRead);
              }
       }

       public void FileRecive(string savePath)
       {
              byte[] jsonLenBytes = new byte[4];
              _networkStream.ReadExactly(jsonLenBytes, 0, 4);
              int jsonLen = BitConverter.ToInt32(jsonLenBytes, 0);

              byte[] jsonBytes = new byte[jsonLen];
              _networkStream.ReadExactly(jsonBytes, 0, jsonLen);

              string json = System.Text.Encoding.UTF8.GetString(jsonBytes);
              _filedata = JsonSerializer.Deserialize<FileData>(json);
              
              string fullPath = Path.Combine(savePath, _filedata.FileName);
              _fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
              
              byte[] buffer = new byte[8192];
              long remaining = _filedata.FileSize;
              
              
              while (remaining > 0)
              {
                     int toRead = (int)Math.Min(buffer.Length, remaining);
                     int read = _networkStream.Read(buffer, 0, toRead);
                     if (read == 0)
                            throw new IOException("Connection closed before file fully received");

                     _fileStream.Write(buffer, 0, read);
                     remaining -= read;              
              }
              
       }
}