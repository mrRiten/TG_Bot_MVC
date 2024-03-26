using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using TG_Bot_MVC;

namespace BuildDefaultSchedule
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = $"./json/";

            MainBuild(path, "Числитель");
            MainBuild(path, "Знаменатель");
        }

        private static void MainBuild(string path, string weekOfSchedule)
        { 
            int idWeekOfSchedule = weekOfSchedule == "Числитель" ? 1 : 2; // 1 - числитель, 2 - знаменатель
            foreach (string file in Directory.GetFiles(path, $"*_{idWeekOfSchedule}.json"))
            {
                string[] groups = GetGroups(file);

                string json = File.ReadAllText(file);
                JArray jsonArray = JArray.Parse(json);
                for (int weekday = 1; weekday <= jsonArray.Count; weekday++)
                {
                    JObject defaultScheduleData = (JObject)jsonArray[weekday];
                    WriteToDatabase(groups, defaultScheduleData.ToString(), weekOfSchedule, weekday);
                }
            }
        }

        private static string[] GetGroups(string file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            string[] fileNameSplit = fileName.Split(' ');
            var groups = new List<string>();
            foreach (string item in fileNameSplit)
                groups.Add(item[..item.IndexOf('_')]);

            return groups.ToArray();
        }

        private static void WriteToDatabase(string[] groups, string defaultSchedule, string weekOfSchedule, int weekDay)
        {
            LibraryContext context = new(true);
            var localAPI = new LocalAPI(context);

            //TODO: localAPI.AddDefaultShedule

        }
    }

}