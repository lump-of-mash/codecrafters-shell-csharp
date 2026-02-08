using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal class CommandHandler
{
    internal bool ExecuteCommand(string[] arguments)
    {
        var fileName = arguments[0];
        var filePath = CheckPathFileIsExecutable(fileName);
        if(filePath == null)
        {
            System.Console.WriteLine($"{fileName}: command not found");
            return false;
        }

        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            UseShellExecute = false
        };

        foreach (var arg in arguments[1..])
            psi.ArgumentList.Add(arg);
        
        var process = Process.Start(psi);
        process?.WaitForExit();
        return true;
    }

    private string? CheckPathFileIsExecutable(string fileName)
    {
        string pathVariableName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Path" : "PATH";
        string? path = Environment.GetEnvironmentVariable(pathVariableName) ?? string.Empty;

        foreach (var dir in path.Split(Path.PathSeparator))
        {
            var filePath = Path.Combine(dir, fileName);

            if (File.Exists(filePath) && IsExecutable(filePath))
                return filePath;
        }

        return null;

        static bool IsExecutable(string path)
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

    internal void TypeCommand(string[] arguments, string[] builtinCommands)
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
            var filePath = CheckPathFileIsExecutable(fileName);

            if (!string.IsNullOrEmpty(filePath))
                message = $"{fileName} is {filePath}";
        }

        if (string.IsNullOrEmpty(message)) message = $"{fileName}: not found";

        System.Console.WriteLine(message);
    }
}
