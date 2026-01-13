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

    public void StartListening()
    {
        _server.Start();
    }

    public void HandleClients()
    {
        Byte[] bytes = new Byte[256];

        while (true)
        {
            Console.WriteLine("Waiting for connection...");
            using TcpClient client = _server.AcceptTcpClient();
            Console.WriteLine("Connected!");
            
            NetworkStream stream = client.GetStream();

            int i = 0;

            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                
            }

        }
    }


}