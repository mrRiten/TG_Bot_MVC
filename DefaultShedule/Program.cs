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
            int idWeekOfSchedule = weekOfSchedule == "Числитель" ? 2 : 1; // 1 - числитель, 2 - знаменатель
            foreach (string file in Directory.GetFiles(path, $"*_{idWeekOfSchedule}.json"))
            {
                string[] groups = GetGroups(file);

                string json = File.ReadAllText(file);
                JArray jsonArray = JArray.Parse(json);
                for (int weekday = 0; weekday < jsonArray.Count; weekday++)
                {
                    JObject defaultScheduleData = (JObject)jsonArray[weekday];
                    foreach (string group in groups)
                    {
                        WriteToDatabase(group, defaultScheduleData.ToString(), weekOfSchedule, weekday + 1);
                    }
                    
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

        private static void WriteToDatabase(string group, string defaultSchedule, string weekOfSchedule, int weekDay)
        {
            LibraryContext context = new(true);
            var localAPI = new LocalAPI(context);
            
            localAPI.AddDefaultSchedule(
                        localAPI.TryGetGroupId(group),
                        localAPI.GetWeekOfScheduleId(weekOfSchedule),
                        defaultSchedule,
                        weekDay
                    );
        }
    }

}