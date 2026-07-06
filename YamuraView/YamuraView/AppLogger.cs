using System;
using System.IO;

namespace YamuraView
{
    /// <summary>
    /// Simple app-wide log file, truncated at each application start.
    /// Stored alongside the ini file in %APPDATA%.
    /// </summary>
    internal static class AppLogger
    {
        private static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YamuraView.log");
        private static readonly object lockObj = new object();

        public static void Init()
        {
            lock (lockObj)
            {
                File.WriteAllText(LogFilePath, $"YamuraView log started {DateTime.Now:G}{Environment.NewLine}");
            }
        }

        public static void Log(string message)
        {
            lock (lockObj)
            {
                File.AppendAllText(LogFilePath, $"{DateTime.Now:G}  {message}{Environment.NewLine}");
            }
        }

        public static string ReadAll()
        {
            lock (lockObj)
            {
                return File.Exists(LogFilePath) ? File.ReadAllText(LogFilePath) : "";
            }
        }
    }
}
