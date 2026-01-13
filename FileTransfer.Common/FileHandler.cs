using System.Net.Sockets;

namespace FileTransfer.Common;

public class FileHandler
{
       private string _fileName;
       private int _fileSize;
       private string _fileType;
       NetworkStream _stream;

       //Constructor for sending the file
       public FileHandler(string FilePath)
       {
              FileStream fs = new FileStream(FilePath,FileMode.Open,FileAccess.Read);
              _fileName = Path.GetFileName(FilePath);
              
       }
       
       //Constructor for reading the file
       public FileHandler() {}
       
       public void FileSend()
       {
              
       }

       public void FileRecive()
       {
              
       }
}