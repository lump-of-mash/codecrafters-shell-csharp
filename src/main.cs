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
            if(string.IsNullOrEmpty(command)) continue;
            
            string[] commandSplit = command.Split(" ");

            switch (commandSplit[0])
            {
                case "exit":
                    return;
                case "echo":
                    System.Console.WriteLine(string.Join(" ", commandSplit[1..]));
                    break;
                case "type":
                    commandHandler.TypeCommand(commandSplit, builtinCommands);
                    break;
                default:
                    if(string.IsNullOrEmpty(commandSplit[0]))
                        System.Console.WriteLine($"{command}: command not found");
                    else
                        commandHandler.ExecuteCommand(commandSplit);

                    break;
            }

        }
    }
}
