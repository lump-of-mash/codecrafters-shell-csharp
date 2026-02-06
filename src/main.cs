class Program
{
    static void Main()
    {
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
                    string message = commandSplit[1];
                    message += builtinCommands.Contains(commandSplit[1]) ? " is a shell builtin" : " not found";
                    System.Console.WriteLine(message);
                    break;
                default:
                    System.Console.WriteLine($"{command}: command not found");
                    break;
            }

        }
    }
}
