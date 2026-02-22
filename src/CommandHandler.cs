using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal class CommandHandler
{
    private const string HISTORY_LOAD_FLAG = "-r";
    private const string HISTORY_WRITE_FLAG = "-w";
    private const string HISTORY_APPEND_FLAG = "-a";

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
        if (HasFlag(arguments, HISTORY_LOAD_FLAG))
        {
            LoadHistoryFromFile(history, arguments);
            return;
        }

        if (HasFlag(arguments, HISTORY_WRITE_FLAG))
        {
            WriteHistoryToFile(history, arguments);
            return;
        }

        if (HasFlag(arguments, HISTORY_APPEND_FLAG))
        {
            AppendHistoryToFile(history, arguments);
            return;
        }

        var startIndex = GetStartIndex(history, arguments);
        PrintHistory(history, startIndex);
    }

    private static void AppendHistoryToFile(List<string> history, List<string> arguments)
    {
        var fileName = arguments.Count > 2 ? arguments[2] : null;
        if (fileName == null) return;

        var appendFromIndex = FindLastAppendIndex(history);
        File.AppendAllLines(fileName, history[appendFromIndex..]);
    }

    private static int FindLastAppendIndex(List<string> history) => history[..^1].FindLastIndex(h => h.Contains($"history {HISTORY_APPEND_FLAG}")) + 1;

    private static void WriteHistoryToFile(List<string> history, List<string> arguments)
    {
        var fileName = arguments.Count > 2 ? arguments[2] : null;

        if (fileName != null)
            File.WriteAllLines(fileName, history);
    }

    private static bool HasFlag(List<string> arguments, string flag) => arguments.Count > 1 && arguments[1] == flag;

    private static int GetStartIndex(List<string> history, List<string> arguments)
    {
        if (arguments.Count > 1 && int.TryParse(arguments[1], out int limit))
            return Math.Max(history.Count - limit, 0);

        return 0;
    }

    private static void PrintHistory(List<string> history, int limit)
    {
        for (int i = limit; i < history.Count; i++)
            System.Console.WriteLine($"    {i + 1}  {history[i]}");
    }

    private static void LoadHistoryFromFile(List<string> history, List<string> arguments)
    {
        string? filePath = arguments.Count > 2 ? arguments[2] : null;

        if (filePath != null && File.Exists(filePath))
            history.AddRange(File.ReadAllLines(filePath));
    }

    internal static List<string> LoadHistoryFromHISTFILE()
    {
        string? filePath = Environment.GetEnvironmentVariable("HISTFILE");
        if (filePath == null) return [];

        return File.ReadAllLines(filePath).ToList();
    }
}
