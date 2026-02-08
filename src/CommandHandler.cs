using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal class CommandHandler
{
    internal void Type(string[] arguments, string[] builtinCommands)
    {
        if (arguments.Length < 2)
        {
            System.Console.WriteLine($" not found");
            return;
        }

        string fileName = arguments[1];
        string? message = "";


        if (builtinCommands.Contains(fileName))
        {
            message = $"{fileName} is a shell builtin";
        }
        else
        {
            string pathVariableName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Path" : "PATH";
            string? path = Environment.GetEnvironmentVariable(pathVariableName) ?? string.Empty;

            foreach (var dir in path.Split(Path.PathSeparator))
            {
                var filePath = Path.Combine(dir, fileName);

                if (File.Exists(filePath) && IsExecutable(filePath))
                {
                    message = $"{fileName} is {filePath}";
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(message)) message = $"{fileName}: not found";

        System.Console.WriteLine(message);


        bool IsExecutable(string path)
        {
            if (!File.Exists(path)) return false;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string ext = Path.GetExtension(path).ToLowerInvariant();
                return ext is ".exe" or ".bat" or ".cmd" or ".com";
            }
            else
            {
                var mode = File.GetUnixFileMode(path);
                return (mode & (UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute)) != 0;
            }
        }

    }
}
