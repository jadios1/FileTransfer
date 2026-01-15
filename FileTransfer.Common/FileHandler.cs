using System.Net.Sockets;
using System.Text.Json;

namespace FileTransfer.Common;

public class FileHandlerAsync
{
       private FileData? _filedata;
       private NetworkStream _networkStream;

       public FileHandlerAsync(NetworkStream networkStream)
       {
              _networkStream = networkStream;
       }
       
       public async Task FileSendAsync(string filePath,CancellationToken cts)
       {
              var fileStream = new FileStream(filePath,FileMode.Open,FileAccess.Read);
              var info = new FileInfo(filePath);
              
              _filedata = new FileData
              {
                     FileName = Path.GetFileName(filePath),
                     FileSize = info.Length,
                     FileType = Path.GetExtension(filePath)
              };

              
              
              string jsonString = JsonSerializer.Serialize(_filedata);
              byte[] jsonStringBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

              byte [] jsonStringLenBytes = BitConverter.GetBytes(jsonStringBytes.Length);
              
              int jsonLen = jsonStringBytes.Length;
              await _networkStream.WriteAsync(jsonStringLenBytes, 0, jsonStringLenBytes.Length, cts);
              await _networkStream.WriteAsync(jsonStringBytes, 0, jsonStringBytes.Length, cts);
              
              byte[] buffer = new byte[8192];
              int bytesRead;
              
              while ((bytesRead =await fileStream.ReadAsync(buffer, 0, buffer.Length, cts)) > 0)
              {
                     await _networkStream.WriteAsync(buffer, 0, bytesRead, cts);
              }
       }

       public async Task FileReceiveAsync(string savePath,CancellationToken ctk)
       {
              byte[] jsonLenBytes = new byte[4];
              await _networkStream.ReadExactlyAsync(jsonLenBytes, 0, 4,ctk);
              int jsonLen = BitConverter.ToInt32(jsonLenBytes, 0);

              byte[] jsonBytes = new byte[jsonLen];
              await _networkStream.ReadExactlyAsync(jsonBytes, 0, jsonLen, ctk);

              string json = System.Text.Encoding.UTF8.GetString(jsonBytes);
              _filedata = JsonSerializer.Deserialize<FileData>(json);
              
              string fullSavePath = Path.Combine(savePath, _filedata.FileName);
              var fileStream = new FileStream(fullSavePath, FileMode.Create, FileAccess.Write);
              
              byte[] buffer = new byte[8192];
              long remaining = _filedata.FileSize;
              
              
              while (remaining > 0)
              {
                     int toRead = (int)Math.Min(buffer.Length, remaining);
                     int read = await _networkStream.ReadAsync(buffer, 0, toRead, ctk);
                     if (read == 0)
                            throw new IOException("Connection closed before file fully received");

                     await fileStream.WriteAsync(buffer, 0, read, ctk);
                     remaining -= read;
              }
              
       }
}