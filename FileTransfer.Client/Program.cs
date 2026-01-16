namespace FileTransfer.Client;

public static class Program
{
    public static async Task Main()
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        
        Console.Write("Podaj IP serwera: ");
        string ipInput = Console.ReadLine();
        
        Console.Write("Podaj port serwera: ");
        int port = int.Parse(Console.ReadLine()!);
        
        FileTransferClient fileTransferServer = new FileTransferClient(port,ipInput);

        while (true)
        {
            await fileTransferServer.ReadComand(cts.Token);    
        }
        
    }
}