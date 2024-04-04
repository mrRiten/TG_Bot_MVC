using Mysqlx.Session;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using TG_Bot_MVC;
using static System.Net.Mime.MediaTypeNames;

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
            try
            {
                DayOfWeek oldDayOfWeek = GetOldDayOfWeek();
                if (oldDayOfWeek != DayOfWeek.Sunday)
                    _localAPI.DelCorrectSchedules((int)oldDayOfWeek);

                _localAPI.DelCorrectSchedules((int)ParserHTML.CurrentDayOfWeek);

                for (int idGroup = 1; idGroup <= _localAPI.GetMaxIdGroup(); idGroup++)
                {
                    DefaultSchedule? defaultLesson = _localAPI.GetDefaultSchedule(idGroup, (int)DateTime.Today.DayOfWeek);
                    ReplasementLesson? replasementLesson = _localAPI.GetReplasementLesson(idGroup, (int)DateTime.Today.DayOfWeek);
                    if (defaultLesson == null)
                        continue;

                    Dictionary<int, string> defaultData = JsonConvert.DeserializeObject<Dictionary<int, string>>(defaultLesson.SerializeDataLessons);
                    Dictionary<int, string> replaceData = null;

                    if (replasementLesson != null)
                        replaceData = JsonConvert.DeserializeObject<Dictionary<int, string>>(replasementLesson.SerializeDataLessons);

                    Dictionary<int, string> currentScheduleData = BuildCurrentSchedule(replaceData, defaultData);

                    string currentSchedule = BuildUI(currentScheduleData);

                    Logger.LogInfo($"Build comleted for idGroup: {idGroup}");

                    WriteToDatabase(idGroup, currentSchedule);

                    Logger.LogInfo($"Current schedule writed to database for idGroup: {idGroup}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Произошла неизвестная ошибка в Parser: {ex.Message}");
            }
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

        private static DayOfWeek GetOldDayOfWeek()
        {
            DayOfWeek oldDayOfWeek;
            if (ParserHTML.CurrentDayOfWeek == DayOfWeek.Sunday)
                oldDayOfWeek = DayOfWeek.Friday;
            else if (ParserHTML.CurrentDayOfWeek == DayOfWeek.Monday)
                oldDayOfWeek = DayOfWeek.Saturday;
            else
                oldDayOfWeek = ParserHTML.CurrentDayOfWeek - 2;

            return oldDayOfWeek;
        }
        private static string BuildUI(Dictionary<int, string> currentScheduleData)
        {
            string currentSchedule = $"📑 Расписание на {ParserHTML.CurrentDayOfWeek}\n";
            foreach (var el in currentScheduleData)
            {
                int numberSchedule = el.Key;
                string schedulesText = el.Value;
                if (schedulesText == null)
                    continue;

                string pattern = @"^(.*?) - (.*?)( \(❗|$)";
                string replacement = "$1 - [$2]$3";

                string modifiedChedulesText = Regex.Replace(schedulesText, pattern, replacement);

                currentSchedule += $"`{numberSchedule}` **{modifiedChedulesText}**\n";
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
                    (int)ParserHTML.CurrentDayOfWeek
            );
        }
    }
}
