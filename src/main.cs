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

            string[] commandSplit = ParseInput(command);

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
                    commandHandler.ExecuteCommand(commandSplit);
                    break;
            }

        }
    }

    static string[] ParseInput(string input)
    {
        var parsedInput = input.Trim().Split(" ");

        for (int i = 0; i < parsedInput.Length; i++)
        {
            parsedInput[i] = parsedInput[i].Trim().Replace("\'", "");
        }

        return parsedInput;
    }
}
