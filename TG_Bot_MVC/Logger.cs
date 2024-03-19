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
            var maxAge = TimeSpan.FromDays(7);
            LogMessage("Debug.txt", message, maxAge);
        }

        public static void LogInfo(string message)
        {
            var maxAge = TimeSpan.FromDays(7);
            LogMessage("Info.txt", message, maxAge);
        }

        public static void LogWarning(string message)
        {
            var maxAge = TimeSpan.FromDays(7);
            LogMessage("Warning.txt", message, maxAge);
        }

        public static void LogError(string message)
        {
            var maxAge = TimeSpan.FromDays(7);
            LogMessage("Error.txt", message, maxAge);
        }

        private static void LogMessage(string fileName, string message, TimeSpan maxAge)
        {
            string filePath = Path.Combine(_logDirectory, fileName);
            string logMessage = $"{DateTime.Now} : {message}\n";

            RemoveOldLog(filePath, maxAge);
            File.AppendAllText(filePath, logMessage);
        }
        private static void RemoveOldLog(string filePath, TimeSpan maxAge)
        {
            if (File.Exists(filePath))
            {
                DateTime creationTime = File.GetCreationTime(filePath);
                if (creationTime - DateTime.Now >= maxAge)
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
