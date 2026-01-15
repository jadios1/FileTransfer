using System.Net;
using System.Net.Sockets;

namespace FileTransfer.Server;

public static class Program
{
    public static async Task Main()
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        
        Console.Write("Podaj IP serwera: ");
        string ipInput = Console.ReadLine();

        Console.Write("Podaj port serwera: ");
        int port = int.Parse(Console.ReadLine()!);
        
        FileTransferServer fileTransferServer = new FileTransferServer(port,ipInput);
        
        var waitForAnyKey = Task.Run(() =>
        {
            Console.WriteLine("Wait any key to exit");
            Console.ReadKey(true);
            cts.Cancel();
        });

        await fileTransferServer.AcceptClients(cts.Token);
        await waitForAnyKey;
    }


}