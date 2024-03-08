using Newtonsoft.Json.Linq;

namespace TG_Bot_MVC
{
    internal class ConfigWorker
    {
        private readonly string mainConfig = "mainConf.json";

        private JObject GetJsonData()
        {
            var json = File.ReadAllText(mainConfig);
            var josnData = JObject.Parse(json);
            return josnData;
        }

        public string GetDebugConnectionString()
        {
            var data = GetJsonData();
            var connectionString = (string)data["DebugConnectionString"] ?? " ";
            return connectionString;
        }

        public string GetReleaseConnectionString()
        {
            var data = GetJsonData();
            var connectionString = (string)data["ReleaseConnectionString"] ?? " ";
            return connectionString;
        }

        public string GetLoggerString()
        {
            var data = GetJsonData();
            var loggerString = (string)data["LoggerString"] ?? " ";
            return loggerString;
        }

        public string GetBotToken()
        {
            var data = GetJsonData();
            var botToken = (string)data["BotToken"] ?? " ";
            return botToken;
        }

        public string[] GetAdmins()
        {
            var data = GetJsonData();
            var adminsJson = (JArray)data["Admins"];
            string[] admins = adminsJson.ToObject<string[]>();
            return admins;
        }

        public string GetHelloMessage()
        {
            var data = GetJsonData();
            var helloMessage = (string)data["HelloMessage"] ?? " ";
            return helloMessage;
        }

    }

}
