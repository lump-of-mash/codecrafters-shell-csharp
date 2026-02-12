using System.Xml;

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

            bool redirectOutput = CheckForRedirect(arguments, out string redirectPath);

            string commandOutput = string.Empty;
            switch (arguments[0])
            {
                case "exit":
                    return;
                case "echo":
                    commandOutput = string.Join(" ", arguments[1..]);
                    OutputCommand(commandOutput, redirectOutput, redirectPath);
                    break;
                case "type":
                    commandOutput = commandHandler.TypeCommand(arguments.ToArray(), builtinCommands);
                    OutputCommand(commandOutput, redirectOutput, redirectPath);
                    break;
                default:
                    commandOutput = commandHandler.ExecuteCommand(arguments.ToArray());
                    OutputCommand(commandOutput, redirectOutput, redirectPath);
                    break;
            }

        }
    }

    private static void OutputCommand(string commandOutput, bool redirectOutput, string redirectPath)
    {
        if (string.IsNullOrWhiteSpace(commandOutput)) return;

        if (redirectOutput)
            File.WriteAllText(redirectPath, commandOutput);
        else
            System.Console.WriteLine(commandOutput.TrimEnd('\n'));
    }

    private static bool CheckForRedirect(List<string> arguments, out string redirectPath)
    {
        if (arguments.Contains(">"))
        {
            var redirectIndex = arguments.IndexOf(">");
            redirectPath = arguments[redirectIndex + 1];
            arguments.RemoveRange(redirectIndex, arguments.Count - redirectIndex);
            return true;
        }
        redirectPath = string.Empty;
        return false;
    }

    static List<string> ParseInput(string input)
    {
        List<string> arguments = [];

        if (string.IsNullOrWhiteSpace(input)) return arguments;

        bool inSingleQuotes = false;
        bool inDoubleQuotes = false;
        string currentArgument = string.Empty;
        bool blackslashEscape = false;
        foreach (var currentChar in input)
        {
            if (blackslashEscape)
            {
                if (inDoubleQuotes && !(currentChar is '\"' or '\\' or '$' or '`' or '\n'))
                    currentArgument += "\\" + currentChar;
                else
                    currentArgument += currentChar;

                blackslashEscape = false;
                continue;
            }

            if (currentChar == '\\' && !inSingleQuotes)
            {
                blackslashEscape = true;
                continue;
            }

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
