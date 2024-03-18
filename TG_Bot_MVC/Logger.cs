namespace TG_Bot_MVC
{
    public static class Logger
    {
        private static readonly string _logDirectory = "logs";

        static Logger()
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public static void LogDebug(string message)
        {
            LogMessage("Debug.txt", message);
        }

        public static void LogInfo(string message)
        {
            LogMessage("Info.txt", message);
        }

        public static void LogWarning(string message)
        {
            LogMessage("Warning.txt", message);
        }

        public static void LogError(string message)
        {
            LogMessage("Error.txt", message);
        }

        private static void LogMessage(string fileName, string message)
        {
            string filePath = Path.Combine(_logDirectory, fileName);
            string logMessage = $"{DateTime.Now} : {message}\n";

            RemoveOldLog(filePath);
            File.AppendAllText(filePath, logMessage);
        }
        private static void RemoveOldLog(string filePath)
        {
            if (File.Exists(filePath))
            {
                DateTime creationTime = File.GetCreationTime(filePath);
                var maxAge = TimeSpan.FromDays(7);
                if (creationTime - DateTime.Now >= maxAge)
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
