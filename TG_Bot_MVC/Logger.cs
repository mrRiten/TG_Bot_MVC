using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG_Bot_MVC
{
    public static class Logger
    {
        private static readonly string pathDebug = "Debug.txt";

        private static readonly string pathInfo = "Log.txt";

        private static readonly string pathWarning = "Warning.txt";

        private static readonly string pathError = "Error.txt";

        public static void LogDebug(string message)
        {
            File.AppendAllText(pathDebug, $"{DateTime.Now} : {message}\n");
        }
        public static void LogInfo(string message)
        {
            File.AppendAllText(pathInfo, $"{DateTime.Now} : {message}\n");
        }
        public static void LogWarning(string message)
        {
            File.AppendAllText(pathWarning, $"{DateTime.Now} : {message}\n");
        }
        public static void LogError(string message)
        {
            File.AppendAllText(pathError, $"{DateTime.Now} : {message}\n");
        }
    }
}
