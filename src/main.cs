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

        var parsedInput = input.Trim();

        bool newArgument;
        string argument;
        int l = 0;
        for (int r = 0; r < parsedInput.Length; r++)
        {
            newArgument = false;

            if (parsedInput[r] == ' ' && parsedInput[l] != '\'')
            {
                newArgument = true;
            }
            else if (parsedInput[r] == '\'')
            {
                if (r + 1 < parsedInput.Length && parsedInput[r + 1] == '\'')
                    r++;
                else if (parsedInput[l] == '\'')
                    newArgument = true;
            }

            if (newArgument)
            {
                argument = parsedInput[(parsedInput[l] == '\'' ? l + 1 : l)..r];
                arguments.Add(argument);
                l = r + 1;
                while (l + 1 < parsedInput.Length && parsedInput[l] == ' ')
                {
                    l++;
                }
                r = l;
            }
        }
        if (l < parsedInput.Length)
        {
            arguments.Add(parsedInput[l..parsedInput.Length].Trim());
        }

        for (int i = 0; i < arguments.Count; i++)
        {
            arguments[i] = arguments[i].Replace("\'", "");
        }

        return arguments;
    }
}
