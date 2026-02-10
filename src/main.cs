using System.Globalization;

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
            if (string.IsNullOrEmpty(command)) continue;

            List<string> arguments = ParseInput(command);

            switch (arguments[0])
            {
                case "exit":
                    return;
                case "echo":
                    System.Console.WriteLine(string.Join(" ", arguments[1..]));
                    break;
                case "type":
                    commandHandler.TypeCommand(arguments.ToArray(), builtinCommands);
                    break;
                default:
                    commandHandler.ExecuteCommand(arguments.ToArray());
                    break;
            }

        }
    }

    static List<string> ParseInput(string input)
    {
        List<string> arguments = new();

        if(string.IsNullOrWhiteSpace(input)) return arguments;

        bool inSingleQuotes = false;
        bool inDoubleQuotes = false;
        string currentArgument = string.Empty;
        foreach (var currentChar in input)
        {
            if (currentChar == '\'' && !inDoubleQuotes)
            {
                inSingleQuotes = !inSingleQuotes;
                continue;
            }

            if (currentChar == '\"' && !inSingleQuotes)
            {
                inDoubleQuotes = !inDoubleQuotes;
                continue;
            }

            if (currentChar == ' ' && !inSingleQuotes && !inDoubleQuotes)
            {
                if (currentArgument.Length > 0)
                {
                    arguments.Add(currentArgument);
                    currentArgument = string.Empty;
                }
            }
            else
            {
                currentArgument += currentChar;
            }
        }
        if (currentArgument.Length > 0) arguments.Add(currentArgument);

        return arguments;
    }
}
