using System.Runtime.InteropServices.Marshalling;
using System.Xml;

class Program
{
    static void Main()
    {
        CommandHandler commandHandler = new();

        string[] builtinCommands = ["echo", "exit", "type"];
        string[] standardRedirectOperators = [">", "1>"];
        string[] errorRedirectOperators = ["2>"];
        string[] appendStandardOperators = [">>", "1>>"];
        string[] appendErrorOperators = ["2>>"];
        while (true)
        {
            Console.Write("$ ");

            string? command = Console.ReadLine();
            if (string.IsNullOrEmpty(command)) continue;

            List<string> arguments = ParseInput(command);

            bool redirectStandardOutput = CheckForRedirect(arguments, standardRedirectOperators, out string standardRedirectPath);
            bool appendStandardOutput = CheckForRedirect(arguments, appendStandardOperators, out string appendStandardPath, true);
            bool redirectErrorOutput = CheckForRedirect(arguments, errorRedirectOperators, out string errordRedirectPath);
            bool appendErrorOutput = CheckForRedirect(arguments, appendErrorOperators, out string appendErrorPath, true);

            string commandOutput = string.Empty;
            string errorOutput = string.Empty;
            switch (arguments[0])
            {
                case "exit":
                    return;
                case "echo":
                    commandOutput = string.Join(" ", arguments[1..]);
                    break;
                case "type":
                    commandOutput = commandHandler.TypeCommand(arguments.ToArray(), builtinCommands);
                    break;
                default:
                    (commandOutput, errorOutput) = commandHandler.ExecuteCommand(arguments.ToArray());
                    break;
            }

            // Handle output for all commands
            if (!string.IsNullOrWhiteSpace(commandOutput))
            {
                commandOutput += "\n";

                if (appendStandardOutput)
                    File.AppendAllText(appendStandardPath, commandOutput);
                else
                    OutputCommand(commandOutput, redirectStandardOutput, standardRedirectPath);
            }
            if (!string.IsNullOrWhiteSpace(errorOutput))
            {
                if (appendErrorOutput)
                    File.AppendAllText(appendErrorPath, errorOutput);
                else
                    OutputCommand(errorOutput, redirectErrorOutput, errordRedirectPath);
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

    private static bool CheckForRedirect(List<string> arguments, string[] redirectOperators, out string redirectPath, bool append = false)
    {
        for (int i = 1; i < arguments.Count - 1; i++)
        {
            if (redirectOperators.Contains(arguments[i]))
            {
                redirectPath = arguments[i + 1];
                arguments.RemoveRange(i, arguments.Count - i);

                if (append)
                    File.AppendAllText(redirectPath, null);
                else
                    File.WriteAllText(redirectPath, null);

                return true;
            }
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
