using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal class CommandHandler
{
    internal void Type(string[] command, string[] builtinCommands)
    {
        string fileName = command[1];
        string? message = "";

        if (builtinCommands.Contains(fileName))
        {
            message = $"{fileName} is a shell builtin";
        }
        else
        {
            string? path = Environment.GetEnvironmentVariable("Path");

            foreach (var dir in path.Split(';'))
            {
                try
                {
                    var dirFiles = Directory.GetFiles(dir);
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }
                
                var filePath = Path.Combine(dir, fileName);

                if (File.Exists(filePath) && IsExecutable(filePath))
                {
                    message = $"{fileName} is {filePath}";
                    break;
                }
            }
        }

        if (message == "") message = $"{fileName}: not found";

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
