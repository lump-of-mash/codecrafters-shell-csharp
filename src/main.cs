class Program
{
    static void Main()
    {
        CommandHandler commandHandler = new();

        string[] builtinCommands = ["echo", "exit", "type"];
        while (true)
        {
            Console.Write("$ ");

            string? command = Console.ReadLine();
            string[] commandSplit = command.Split(" ");

            switch (commandSplit[0])
            {
                case "exit":
                    return;
                case "echo":
                    System.Console.WriteLine(string.Join(" ", commandSplit[1..]));
                    break;
                case "type":
                    commandHandler.Type(commandSplit, builtinCommands);
                    break;
                default:
                    System.Console.WriteLine($"{command}: command not found");
                    break;
            }

        }
    }

    private void Type(string[] command)
    {
        
    } 
}
