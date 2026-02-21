using System.Runtime.InteropServices;

/// <summary>
/// Directly controls PTY echo mode via POSIX termios so that:
/// - The PTY does NOT echo input back to stdout (avoiding double-echo with our manual echo)
/// - The setting is applied atomically to our own stdin fd, with no subprocess race condition
/// </summary>
internal static class TerminalSettings
{
    // termios struct layout for Linux x86_64 / arm64
    // See: /usr/include/bits/termios.h
    [StructLayout(LayoutKind.Sequential)]
    private struct Termios
    {
        public uint c_iflag;   // input flags
        public uint c_oflag;   // output flags
        public uint c_cflag;   // control flags
        public uint c_lflag;   // local flags  ← ECHO lives here
        public byte c_line;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] c_cc;    // control characters
        public uint c_ispeed;
        public uint c_ospeed;
    }

    [DllImport("libc", SetLastError = true)]
    private static extern int tcgetattr(int fd, out Termios termios);

    [DllImport("libc", SetLastError = true)]
    private static extern int tcsetattr(int fd, int optionalActions, ref Termios termios);

    private const int STDIN_FILENO = 0;
    private const int TCSANOW = 0;   // apply change immediately
    private const uint ECHO = 0x8;   // echo input characters

    private static Termios _original;
    private static bool _saved;

    /// <summary>
    /// Turns off the PTY's built-in echo. Call once at startup on non-Windows.
    /// Our InputReader manually echoes characters itself, so we don't want the
    /// PTY to also echo them — that causes every character to appear twice.
    /// </summary>
    public static void DisableEcho()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
        if (Console.IsInputRedirected) return;

        if (tcgetattr(STDIN_FILENO, out _original) != 0) return;
        _saved = true;

        var raw = _original;
        raw.c_lflag &= ~ECHO;  // clear the ECHO bit
        tcsetattr(STDIN_FILENO, TCSANOW, ref raw);
    }

    /// <summary>
    /// Restores the terminal to its original state. Call before exit.
    /// </summary>
    public static void RestoreEcho()
    {
        if (!_saved) return;
        tcsetattr(STDIN_FILENO, TCSANOW, ref _original);
    }
}