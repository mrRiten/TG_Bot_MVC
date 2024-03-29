using Newtonsoft.Json;
using TG_Bot_MVC;

namespace Parser
{
    internal class ScheduleBuilder
    {
        private readonly LocalAPI _localAPI;
        public ScheduleBuilder()
        {
            _localAPI = new LocalAPI(new LibraryContext());
        }

        public void MainBuild()
        {
            _localAPI.DelCorrectSchedules((int)DateTime.Today.DayOfWeek);
            for (int idGroup = 1; idGroup <= _localAPI.GetMaxIdGroup(); idGroup++)
            {
                DefaultSchedule? defaultLesson = _localAPI.GetDefaultSchedule(idGroup, (int)DateTime.Today.DayOfWeek);
                ReplasementLesson? replasementLesson = _localAPI.GetReplasementLesson(idGroup, (int)DateTime.Today.DayOfWeek);
                if (defaultLesson == null)
                    continue;
                Dictionary<int, string> defaultData = JsonConvert.DeserializeObject<Dictionary<int, string>>(defaultLesson.SerializeDataLessons);
                Dictionary<int, string> replaceData = null;
                if (replasementLesson != null)
                {
                    replaceData = JsonConvert.DeserializeObject<Dictionary<int, string>>(replasementLesson.SerializeDataLessons);
                }

                Dictionary<int, string> currentScheduleData = BuildCurrentSchedule(replaceData, defaultData);

                string currentSchedule = BuildUI(currentScheduleData);

                WriteToDatabase(idGroup, currentSchedule);
            }
            Console.WriteLine("Buuild comleted");
        }
        private static Dictionary<int, string> BuildCurrentSchedule(Dictionary<int, string> replaceData, Dictionary<int, string> defaultData)
        {
            if (replaceData != null)
            {
                for (int i = 0; i < defaultData.Count; i++)
                {
                    if (replaceData[i] != null)
                    {
                        defaultData[i] = replaceData[i];
                    }
                }
            }
            return defaultData;
        }
        private static string BuildUI(Dictionary<int, string> currentScheduleData) {
            string currentSchedule = $"📑 Расписание на {DateTime.Today.DayOfWeek}\n";
            foreach (var item in currentScheduleData)
            {
                if (item.Value == null)
                    continue;
                string key = item.Key.ToString();
                string value = item.Value;

                string result = value;
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i] == '-')
                    {
                        result = result.Insert(i + 2, "[");
                    }
                    else if (result[i] == '(')
                    {
                        result = result.Insert(i - 2, "]");
                    }
                    else if (i + 1 == value.Length)
                    {
                        result = result.Insert(i + 2, "]");
                    }
                    
                }
                currentSchedule += $"`{key}` **{result}**\n";
            }
            return currentSchedule;
        }
        private void WriteToDatabase(int idGroup, string currentSchedule)
        {
            Logger.LogDebug($"\n{currentSchedule}");
            _localAPI.AddCorrectSchedule(
                    idGroup,
                    _localAPI.GetWeekOfScheduleId(ParserHTML.WeekOfSchedule),
                    currentSchedule,
                    (int)DateTime.Today.DayOfWeek
            );
        }
    }
}
