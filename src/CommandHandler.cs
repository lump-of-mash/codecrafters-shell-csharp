using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal class CommandHandler
{
    internal (string output, string error) ExecuteCommand(string[] arguments)
    {
        var fileName = arguments[0];
        var filePath = CheckPathFileIsExecutable(fileName);
        if(filePath == null)
        {
            return ($"{fileName}: command not found", string.Empty);
        }
        
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
            FileName = fileName,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
            }
        };
        foreach (var arg in arguments[1..])
        {
            process.StartInfo.ArgumentList.Add(arg);
        }
        process.Start();
        
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        process.WaitForExit();
        return (output, error);
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

    internal string TypeCommand(string[] arguments, string[] builtinCommands)
    {
        if (arguments.Length < 2)
        {
            return $" not found";
        }

        string fileName = arguments[1];
        string? message = "";


        if (builtinCommands.Contains(fileName))
        {
            return $"{fileName} is a shell builtin";
        }
        else
        {
            var filePath = CheckPathFileIsExecutable(fileName);

            if (!string.IsNullOrEmpty(filePath))
                message = $"{fileName} is {filePath}";
        }

        if (string.IsNullOrEmpty(message)) message = $"{fileName}: not found";

        return message;
    }
}
