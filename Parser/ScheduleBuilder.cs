using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Security.Policy;
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
            for (int idGroup = 1; idGroup < 86; idGroup++)
            {
                string defaultLesson = "{\r\n  \"0\": \"123\",\r\n  \"1\": \"123\",\r\n  \"2\": \"123\",\r\n  \"3\": \"123\",\r\n  \"4\": \"123\",\r\n  \"5\": \"123\",\r\n  \"6\": \"123\"\r\n}";
                //DefaultSchedule? defaultLesson = _localAPI.GetDefaultSchedule(i, (int)DateTime.Today.DayOfWeek);
                ReplasementLesson? replasementLesson = _localAPI.GetReplasementLesson(idGroup, (int)DateTime.Today.DayOfWeek);

                Dictionary<int, string> defaultData = JsonConvert.DeserializeObject<Dictionary<int, string>>(defaultLesson);
                Dictionary<int, string> replaceData = null;
                if (replasementLesson != null)
                {
                    replaceData = JsonConvert.DeserializeObject<Dictionary<int, string>>(replasementLesson.SerializeDataLessons);
                }

                if (replaceData != null)
                { 
                    for (int j = 0; j < defaultData.Count; j++)
                    {
                        if (replaceData[j] != null)
                        {
                            defaultData[j] = replaceData[j];
                        }
                    }
                }

                string currentData = JsonConvert.SerializeObject(defaultData, Formatting.Indented);

                WriteToDatabase(idGroup, currentData);
            }
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
