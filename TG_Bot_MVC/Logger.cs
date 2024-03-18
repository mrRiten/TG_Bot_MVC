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
            int maxLinesDebug = 10000;
            LogMessage("Debug.txt", message, maxLinesDebug);
        }

        public static void LogInfo(string message)
        {
            int maxLinesInfo = 1000;
            LogMessage("Info.txt", message, maxLinesInfo);
        }

        public static void LogWarning(string message)
        {
            int maxLinesWarning = 500;
            LogMessage("Warning.txt", message, maxLinesWarning);
        }

        public static void LogError(string message)
        {
            int maxLinesError = int.MaxValue;
            LogMessage("Error.txt", message, maxLinesError);
        }

        private static void LogMessage(string fileName, string message, int maxLines)
        {
            string filePath = Path.Combine(_logDirectory, fileName);
            string logMessage = $"{DateTime.Now} : {message}\n";
            File.AppendAllText(filePath, logMessage);

            RemoveOldLines(filePath, maxLines);
        }
        private static void RemoveOldLines(string filePath, int maxLines)
        {
            string[] lines = File.ReadAllLines(filePath);
            int lineCount = lines.Length;
            if (lineCount > maxLines)
            {
                int linesToRemove = lineCount - maxLines;
                File.WriteAllLines(filePath, lines.Skip(linesToRemove));
            }
        }
    }
}
