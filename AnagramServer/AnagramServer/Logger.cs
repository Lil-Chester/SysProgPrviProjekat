using System;
using System.IO;

namespace AnagramServer
{
    public static class Logger
    {
        private static readonly object logLock = new object();

        public static void Log(string message)
        {
            lock (logLock)
            {
                File.AppendAllText(
                    "logs.txt",
                    $"[{DateTime.Now}] {message}\n"
                );
            }
        }
    }
}