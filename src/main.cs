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

        bool inSingleQuotes = false; 
        for (int r = 0; r < parsedInput.Length; r++)
        {
            var currentChar = parsedInput[r];

            if(currentChar == '\'')
                inSingleQuotes = !inSingleQuotes;

            if(currentChar == ' ' && !inSingleQuotes)
            {
                arguments.Add(parsedInput[l..r]);
                l = r + 1;
                while(l + 1 < parsedInput.Length && parsedInput[l] == ' ')
                {
                    l++;
                    r++;
                }
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
