using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal class CommandHandler
{
    internal (string output, string error) ExecuteCommand(string[] arguments)
    {
        var fileName = arguments[0];
        var filePath = CheckPathFileIsExecutable(fileName);
        if (filePath == null)
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

    public static List<string> GetExecutableFileNames()
    {
        List<string> fileNames = [];
        string path = GetPathDirectories();

        foreach (var dir in path.Split(Path.PathSeparator))
        {
            if (!Directory.Exists(dir)) continue;

            foreach (var filePath in Directory.GetFiles(dir))
            {
                if (IsExecutable(filePath))
                    fileNames.Add(Path.GetFileName(filePath));
            }
        }
        return fileNames;
    }

    private static string GetPathDirectories()
    {
        string pathVariableName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Path" : "PATH";
        return Environment.GetEnvironmentVariable(pathVariableName) ?? string.Empty;
    }

    private string? CheckPathFileIsExecutable(string fileName)
    {
        string path = GetPathDirectories();

        foreach (var dir in path.Split(Path.PathSeparator))
        {
            var filePath = Path.Combine(dir, fileName);

            if (File.Exists(filePath) && IsExecutable(filePath))
                return filePath;
        }

        return null;
    }

    private static bool IsExecutable(string path)
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

    internal static string CDCommand(List<string> arguments)
    {
        var directory = string.Empty;
        var output = string.Empty;

        if (arguments.Count > 1)
            directory = arguments[1];

        // use home directory for tilde character
        if (directory == "~")
            directory = Environment.GetEnvironmentVariable("HOME") ?? "~";

        if (Directory.Exists(directory))
            Directory.SetCurrentDirectory(directory);
        else
            output = $"cd: {directory}: No such file or directory";

        return output;
    }

    internal static void HistoryCommand(List<string> history, List<string> arguments)
    {
        int limit = 0;

        if (arguments.Count > 1)
        {
            var secondArgument = arguments[1];

            if (secondArgument == "-r")
            {
                if(arguments.Count > 2 && File.Exists(arguments[2]))
                {
                    foreach(var line in File.ReadAllLines(arguments[2]))
                        history.Add(line);
                }
                return;
            }
            else if (int.TryParse(secondArgument, out limit))
                limit = Math.Max(history.Count - limit, 0);
        }

        for (int i = limit; i < history.Count; i++)
        {
            System.Console.WriteLine($"    {i + 1}  {history[i]}");
        }
    }
}
