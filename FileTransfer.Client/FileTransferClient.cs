using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using FileTransfer.Common;

namespace FileTransfer.Client;

public class FileTransferClient
{
    private Int32 _port;
    private IPAddress _ipAddress;
    private TcpClient _client;
    private NetworkStream _stream;
    
    public FileTransferClient(int port,string ipAddress)
    {
        _port = port;
        _ipAddress = IPAddress.Parse(ipAddress);
        _client = new TcpClient();
    }

    public async Task ConnectAsync(CancellationToken token)
    {
        
        await _client.ConnectAsync(_ipAddress,_port);
        _stream = _client.GetStream();

    }

    public async Task UploadFileAsync(string filePath, CancellationToken token)
    {
        FileHandlerAsync handler = new FileHandlerAsync(_stream);
        await handler.FileSendAsync(filePath, token);

    }

    public async Task DownloadFileAsync(string savePath, CancellationToken token)
    {
        FileHandlerAsync handler = new FileHandlerAsync(_stream);
        await handler.FileReceiveAsync(savePath, token);


    }

    public async Task SendCommand(string commandstring,CancellationToken token)
    {
        Command command = new Command(commandstring);
        var json = JsonSerializer.Serialize(command);
        var jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
        Byte[] jsonLen = BitConverter.GetBytes(jsonBytes.Length);
        
        await _stream.WriteAsync(jsonLen,token);
        await _stream.WriteAsync(jsonBytes,token);
        
    }

    public async Task<Command> ReadComand(CancellationToken token)
    {
        Byte[] jsonLenBytes = new byte[4];
        await _stream.ReadExactlyAsync(jsonLenBytes, 0, 4,token);
        int jsonLen = BitConverter.ToInt32(jsonLenBytes);

        Byte[] jsonBytes = new byte[jsonLen];
        
        await _stream.ReadExactlyAsync(jsonBytes, 0, jsonLen, token);
        
        string json = System.Text.Encoding.UTF8.GetString(jsonBytes);
        Command command = JsonSerializer.Deserialize<Command>(json)!;
        
        return command;
    }

    public void Disconnect()
    {
        _client.Dispose();
    }
    
}