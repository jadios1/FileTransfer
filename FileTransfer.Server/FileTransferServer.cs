using FileTransfer.Common;

namespace FileTransfer.Server;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class FileTransferServer
{
    private Int32 _port;
    private IPAddress _ipAddress;
    private TcpListener _server;

    public FileTransferServer(Int32 port, string ipAddress)
    {
        _port = port;
        _ipAddress = IPAddress.Parse(ipAddress);
        _server = new TcpListener(_ipAddress,_port);
    }



    public async Task AcceptClients(CancellationToken token = default)
    {
        _server.Start();
        var clients = new List<Task>();
        while (true)
        {
            try
            {
                TcpClient client = await _server.AcceptTcpClientAsync(token);
                clients.Add(HandleClientsAsync(client,token));
                clients.RemoveAll(task => task.IsCompleted);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _server.Stop();
        await Task.WhenAll(clients);

    }

    private async Task HandleClientsAsync(TcpClient client,CancellationToken cts)
    {
        try
        {
            Byte[] bytes = new Byte[256];
            NetworkStream stream = client.GetStream();

            FileHandlerAsync handler = new FileHandlerAsync(stream);
            await handler.FileReceiveAsync("/home/jadios/Documents/FileTransferTemp/", cts);

        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Server Shutdown!");
        }
        catch (IOException)
        {
            Console.WriteLine("Client Disconnected abruptly");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error handling client: {e.Message}");
        }
        finally
        {
            client.Dispose();
            Console.WriteLine("Client disconnected");
        }
    }


}