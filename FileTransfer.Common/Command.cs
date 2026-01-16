namespace FileTransfer.Common;

public class Command
{
    public Command(string command)
    {
        string [] commandsplit = command.Split(' ');
        type = commandsplit[0];
        filename = commandsplit[1];
    }
    
    public string type;
    public string filename;
    public string command;

}