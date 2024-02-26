using Newtonsoft.Json;

namespace TG_Bot_MVC
{
    internal static class Serializer
    {
        public static string SerializeJson(object data)
        {
            return JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
        }
        public static object DeSerializeJson(string data)
        {
            return JsonConvert.DeserializeObject(data);
        }
    }
}
