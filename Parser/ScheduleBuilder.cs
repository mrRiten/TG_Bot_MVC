using Newtonsoft.Json;
using TG_Bot_MVC;
using ZstdSharp.Unsafe;

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
            int weekOfSchedule = _localAPI.GetCorrentWeekOfSchedule();
            for (int idGroup = 1; idGroup <= _localAPI.GetMaxIdGroup(); idGroup++)
            {
                DefaultSchedule? defaultLesson = _localAPI.GetDefaultSchedule(idGroup, weekOfSchedule, (int)DateTime.Today.DayOfWeek);
                ReplasementLesson? replasementLesson = _localAPI.GetReplasementLesson(idGroup, (int)DateTime.Today.DayOfWeek);
                Dictionary<int, string> defaultData = JsonConvert.DeserializeObject<Dictionary<int, string>>(defaultLesson.SerializeDataLessons);
                Dictionary<int, string> replaceData = null;
                if (replasementLesson != null)
                {
                    replaceData = JsonConvert.DeserializeObject<Dictionary<int, string>>(replasementLesson.SerializeDataLessons);
                }

                string currentSchedule = BuildCurrentSchedule(replaceData, defaultData);

                WriteToDatabase(idGroup, currentSchedule);
                Logger.LogInfo("CurrentSchedule was loaded into DataBase");
            }
        }
        private static string BuildCurrentSchedule(Dictionary<int, string> replaceData, Dictionary<int, string> defaultData)
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
            return JsonConvert.SerializeObject(defaultData, Formatting.Indented);
        }
        private void WriteToDatabase(int idGroup, string currentData)
        {
            Logger.LogDebug($"\n{currentData}");
            _localAPI.DelCorrectSchedules((int)DateTime.Today.DayOfWeek);
            _localAPI.AddCorrectSchedule(
                    idGroup,
                    _localAPI.GetWeekOfScheduleId(ParserHTML.WeekOfSchedule),
                    currentData,
                    (int)DateTime.Today.DayOfWeek
            );
        }
    }
}
