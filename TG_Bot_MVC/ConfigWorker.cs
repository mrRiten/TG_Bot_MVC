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

        public string GetConnectionString()
        {
            var data = GetJsonData();
            var connectionString = (string)data["ConnectionString"] ?? " ";
            return connectionString;
        }

        public string GetLoggerString()
        {
            var data = GetJsonData();
            var loggerString = (string)data["LoggerString"] ?? " ";
            return loggerString;
        }

        public string[] GetAdmins()
        {
            var data = GetJsonData();
            var adminsJson = (JArray)data["Admins"];
            string[] admins = adminsJson.ToObject<string[]>();
            return admins;
        }

    }

}
